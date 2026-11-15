using Meta.Integration;
using Meta.Operations;
using Meta.Operations.Domain;

namespace MetaMultiDimensional.Core;

public sealed class MultiDimensionalAuthoringRequest
{
    public required string WorkspacePath { get; init; }
    public required string EntityName { get; init; }
    public required string RecordId { get; init; }
    public Dictionary<string, string> Values { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<MultiDimensionalRelationshipAssignment> Relationships { get; } = new();
}

public sealed record MultiDimensionalRelationshipAssignment(
    string ColumnName,
    string TargetEntityName,
    string TargetRecordId);

public interface IMultiDimensionalAuthoringService
{
    MetaMultiDimensionalModel CreateWorkspace();

    MetaMultiDimensionalModel AddRecord(MultiDimensionalAuthoringRequest request);
}

public sealed class MultiDimensionalAuthoringService : IMultiDimensionalAuthoringService
{
    private const string ModelName = "MetaMultiDimensional";

    public MetaMultiDimensionalModel CreateWorkspace() => MetaMultiDimensionalModel.CreateEmpty();

    public MetaMultiDimensionalModel AddRecord(MultiDimensionalAuthoringRequest request) =>
        AddRecordAsync(request).GetAwaiter().GetResult();

    private static async Task<MetaMultiDimensionalModel> AddRecordAsync(MultiDimensionalAuthoringRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);
        await using var workspace = await WorkspaceSurface.OpenAsync(Path.GetFullPath(request.WorkspacePath)).ConfigureAwait(false);
        var state = await WorkspaceComposition.MaterializeAsync(workspace).ConfigureAwait(false);
        var binding = new MetaMultiDimensionalWorkspaceBinding();
        _ = binding.Read(state);
        var values = new Dictionary<string, string>(request.Values, StringComparer.OrdinalIgnoreCase);
        var relationships = request.Relationships.Select(item => (item.ColumnName, item.TargetEntityName, item.TargetRecordId)).ToList();
        var operations = new List<Operation>();
        if (request.EntityName is nameof(Dimension) or nameof(Cube) or nameof(MeasureGroup) or nameof(Partition))
        {
            AddDefault("StorageMode", "Molap");
            AddDefault("ProcessingMode", "Regular");
            if (request.EntityName == nameof(Dimension)) AddDefault("ProcessingGroup", "ByAttribute");
        }
        void AddDefault(string name, string value)
        {
            if (!values.TryGetValue(name, out var current) || string.IsNullOrWhiteSpace(current)) values[name] = value;
        }
        operations.Add(BuildInsert(state, request.EntityName, request.RecordId, values, relationships));
        var candidate = InMemoryOperations.ApplyBatch(state, operations);
        var model = binding.Read(candidate);
        await workspace.ExecuteAsync(operations).ConfigureAwait(false);
        return model;
    }

    private static Operation BuildInsert(InMemoryWorkspace state, string entityName, string id,
        Dictionary<string, string> values,
        IReadOnlyList<(string ColumnName, string TargetEntityName, string TargetRecordId)> relationships)
    {
        var entity = state.Model.FindEntity(entityName)
            ?? throw new InvalidOperationException($"Entity '{entityName}' was not found in model '{ModelName}'.");
        var rows = state.Instance.RecordsByEntity.TryGetValue(entity.Name, out var existing)
            ? existing.AsEnumerable() : Enumerable.Empty<GenericRecord>();
        if (rows.Any(row => row.Id == id))
            throw new InvalidOperationException($"{entityName} '{id}' already exists.");
        foreach (var value in values)
            if (!entity.Properties.Any(property => string.Equals(property.Name, value.Key, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Entity '{entityName}' does not define property '{value.Key}'.");
        var references = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var assignment in relationships)
        {
            var column = assignment.ColumnName.EndsWith("Id", StringComparison.Ordinal)
                ? assignment.ColumnName : assignment.ColumnName + "Id";
            var relationship = entity.FindRelationshipByColumnName(column)
                ?? throw new InvalidOperationException($"Entity '{entityName}' does not define relationship '{column[..^2]}'.");
            if (!string.Equals(relationship.Entity, assignment.TargetEntityName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Entity '{entityName}' relationship '{column[..^2]}' cannot reference {assignment.TargetEntityName}.");
            references.Add(relationship.GetColumnName(), assignment.TargetRecordId);
        }
        if (entity.Properties.Any(property => property.Name == "Ordinal") && !values.ContainsKey("Ordinal"))
        {
            if (relationships.Count > 0)
            {
                var first = relationships[0];
                var ownerColumn = first.ColumnName.EndsWith("Id", StringComparison.Ordinal) ? first.ColumnName : first.ColumnName + "Id";
                rows = rows.Where(row => row.RelationshipIds.TryGetValue(ownerColumn, out var owner) && owner == first.TargetRecordId);
            }
            var max = rows.Select(row => row.Values.TryGetValue("Ordinal", out var ordinal) && int.TryParse(ordinal, out var parsed) ? parsed : 0)
                .DefaultIfEmpty(0).Max();
            values["Ordinal"] = (max + 10).ToString();
        }
        return new Operation.InsertRecord(entity.Name, id, values, references);
    }


}
