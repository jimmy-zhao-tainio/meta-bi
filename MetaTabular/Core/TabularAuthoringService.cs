using Meta.Core.Domain;
using Meta.Core.Services;

namespace MetaTabular.Core;

public sealed class TabularAuthoringRequest
{
    public required string WorkspacePath { get; init; }
    public required string EntityName { get; init; }
    public required string RecordId { get; init; }
    public Dictionary<string, string> Values { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<TabularRelationshipAssignment> Relationships { get; } = new();
}

public sealed record TabularRelationshipAssignment(
    string ColumnName,
    string TargetEntityName,
    string TargetRecordId);

public interface ITabularAuthoringService
{
    Task<Workspace> AddRecordAsync(TabularAuthoringRequest request, CancellationToken cancellationToken = default);
}

public sealed class TabularAuthoringService : ITabularAuthoringService
{
    private readonly IWorkspaceService workspaceService;
    private readonly ValidationService validationService;

    public TabularAuthoringService()
        : this(new WorkspaceService(), new ValidationService())
    {
    }

    public TabularAuthoringService(IWorkspaceService workspaceService, ValidationService validationService)
    {
        this.workspaceService = workspaceService ?? throw new ArgumentNullException(nameof(workspaceService));
        this.validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    }

    public async Task<Workspace> AddRecordAsync(
        TabularAuthoringRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var workspace = await workspaceService.LoadAsync(workspacePath, searchUpward: false, cancellationToken)
            .ConfigureAwait(false);
        if (!string.Equals(workspace.Model.Name, MetaTabularModels.MetaTabularModelName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Workspace '{workspacePath}' contained model '{workspace.Model.Name}', not '{MetaTabularModels.MetaTabularModelName}'.");
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
        IReadOnlyList<TabularRelationshipAssignment> relationships)
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
}
