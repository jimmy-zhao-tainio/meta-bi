using Meta.Core.Domain;
using Meta.Core.Services;

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
    Task<Workspace> AddRecordAsync(MultiDimensionalAuthoringRequest request, CancellationToken cancellationToken = default);
}

public sealed class MultiDimensionalAuthoringService : IMultiDimensionalAuthoringService
{
    private readonly IWorkspaceService workspaceService;
    private readonly ValidationService validationService;

    public MultiDimensionalAuthoringService()
        : this(new WorkspaceService(), new ValidationService())
    {
    }

    public MultiDimensionalAuthoringService(IWorkspaceService workspaceService, ValidationService validationService)
    {
        this.workspaceService = workspaceService ?? throw new ArgumentNullException(nameof(workspaceService));
        this.validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    }

    public async Task<Workspace> AddRecordAsync(
        MultiDimensionalAuthoringRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var workspace = await workspaceService.LoadAsync(workspacePath, searchUpward: false, cancellationToken)
            .ConfigureAwait(false);
        if (!string.Equals(workspace.Model.Name, MetaMultiDimensionalModels.MetaMultiDimensionalModelName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Workspace '{workspacePath}' contained model '{workspace.Model.Name}', not '{MetaMultiDimensionalModels.MetaMultiDimensionalModelName}'.");
        }

        var entity = workspace.Model.FindEntity(request.EntityName)
            ?? throw new InvalidOperationException($"Entity '{request.EntityName}' was not found in model '{workspace.Model.Name}'.");

        var records = workspace.Instance.GetOrCreateEntityRecords(entity.Name);
        if (records.Any(record => string.Equals(record.Id, request.RecordId, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException($"{entity.Name} '{request.RecordId}' already exists.");
        }

        foreach (var relationship in request.Relationships)
        {
            if (!entity.Relationships.Any(item => string.Equals(item.GetColumnName(), relationship.ColumnName, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"Entity '{entity.Name}' does not define relationship column '{relationship.ColumnName}'.");
            }

            var targetRecords = workspace.Instance.GetOrCreateEntityRecords(relationship.TargetEntityName);
            if (!targetRecords.Any(record => string.Equals(record.Id, relationship.TargetRecordId, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"{relationship.TargetEntityName} '{relationship.TargetRecordId}' was not found.");
            }
        }

        var valuesToAdd = new Dictionary<string, string>(request.Values, StringComparer.OrdinalIgnoreCase);
        AssignOrdinalIfMissing(workspace, entity, valuesToAdd, request.Relationships);
        AssignTargetDefaults(entity, valuesToAdd);

        var recordToAdd = new GenericRecord
        {
            Id = request.RecordId,
        };

        foreach (var value in valuesToAdd)
        {
            recordToAdd.Values[value.Key] = value.Value;
        }

        foreach (var relationship in request.Relationships)
        {
            recordToAdd.RelationshipIds[relationship.ColumnName] = relationship.TargetRecordId;
        }

        records.Add(recordToAdd);

        var validation = validationService.Validate(workspace);
        if (validation.HasErrors)
        {
            throw new InvalidOperationException(
                string.Join(
                    Environment.NewLine,
                    validation.Issues
                        .Where(issue => issue.Severity == IssueSeverity.Error)
                        .Select(issue => $"{issue.Code}: {issue.Message}")));
        }

        await workspaceService.SaveAsync(workspace, cancellationToken: cancellationToken).ConfigureAwait(false);
        return workspace;
    }

    private static void AssignOrdinalIfMissing(
        Workspace workspace,
        GenericEntity entity,
        Dictionary<string, string> values,
        IReadOnlyList<MultiDimensionalRelationshipAssignment> relationships)
    {
        if (!entity.Properties.Any(property => string.Equals(property.Name, "Ordinal", StringComparison.OrdinalIgnoreCase)) ||
            values.ContainsKey("Ordinal"))
        {
            return;
        }

        var parentRelationship = relationships.FirstOrDefault();
        var existingRows = workspace.Instance.GetOrCreateEntityRecords(entity.Name);
        var siblings = parentRelationship == null
            ? existingRows
            : existingRows.Where(row =>
                row.RelationshipIds.TryGetValue(parentRelationship.ColumnName, out var targetId) &&
                string.Equals(targetId, parentRelationship.TargetRecordId, StringComparison.Ordinal));

        var maxOrdinal = siblings
            .Select(row => row.Values.TryGetValue("Ordinal", out var value) && int.TryParse(value, out var parsed)
                ? parsed
                : 0)
            .DefaultIfEmpty(0)
            .Max();

        values["Ordinal"] = (maxOrdinal + 10).ToString();
    }

    private static void AssignTargetDefaults(GenericEntity entity, Dictionary<string, string> values)
    {
        switch (entity.Name)
        {
            case "Dimension":
                AssignDefaultIfSupported(entity, values, "StorageMode", "Molap");
                AssignDefaultIfSupported(entity, values, "ProcessingMode", "Regular");
                AssignDefaultIfSupported(entity, values, "ProcessingGroup", "ByAttribute");
                break;
            case "Cube":
            case "MeasureGroup":
            case "Partition":
                AssignDefaultIfSupported(entity, values, "StorageMode", "Molap");
                AssignDefaultIfSupported(entity, values, "ProcessingMode", "Regular");
                break;
        }
    }

    private static void AssignDefaultIfSupported(
        GenericEntity entity,
        Dictionary<string, string> values,
        string propertyName,
        string defaultValue)
    {
        if (!entity.Properties.Any(property => string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase)) ||
            values.TryGetValue(propertyName, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        values[propertyName] = defaultValue;
    }
}
