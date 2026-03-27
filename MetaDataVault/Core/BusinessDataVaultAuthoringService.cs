using Meta.Core.Domain;
using Meta.Core.Services;

namespace MetaDataVault.Core;

public sealed class BusinessDataVaultAuthoringRequest
{
    public required string WorkspacePath { get; init; }
    public required string EntityName { get; init; }
    public required string RecordId { get; init; }
    public Dictionary<string, string> Values { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<BusinessDataVaultRelationshipAssignment> Relationships { get; } = new();
}

public sealed record BusinessDataVaultRelationshipAssignment(string ColumnName, string TargetEntityName, string TargetRecordId);

public interface IBusinessDataVaultAuthoringService
{
    Task<Workspace> AddRecordAsync(BusinessDataVaultAuthoringRequest request, CancellationToken cancellationToken = default);
}

public sealed class BusinessDataVaultAuthoringService : IBusinessDataVaultAuthoringService
{
    private readonly IWorkspaceService _workspaceService;
    private readonly ValidationService _validationService;

    public BusinessDataVaultAuthoringService()
        : this(new WorkspaceService(), new ValidationService())
    {
    }

    public BusinessDataVaultAuthoringService(IWorkspaceService workspaceService, ValidationService validationService)
    {
        _workspaceService = workspaceService ?? throw new ArgumentNullException(nameof(workspaceService));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    }

    public async Task<Workspace> AddRecordAsync(BusinessDataVaultAuthoringRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var workspace = await _workspaceService.LoadAsync(workspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);
        if (!string.Equals(workspace.Model.Name, MetaDataVaultModels.MetaBusinessDataVaultModelName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Workspace '{workspacePath}' contained model '{workspace.Model.Name}', not '{MetaDataVaultModels.MetaBusinessDataVaultModelName}'.");
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

        var recordToAdd = new GenericRecord
        {
            Id = request.RecordId,
        };

        foreach (var value in request.Values)
        {
            recordToAdd.Values[value.Key] = value.Value;
        }

        foreach (var relationship in request.Relationships)
        {
            recordToAdd.RelationshipIds[relationship.ColumnName] = relationship.TargetRecordId;
        }

        records.Add(recordToAdd);

        ValidateDomainRules(workspace, request);

        var validation = _validationService.Validate(workspace);
        if (validation.HasErrors)
        {
            throw new InvalidOperationException(
                string.Join(Environment.NewLine,
                    validation.Issues
                        .Where(issue => issue.Severity == IssueSeverity.Error)
                        .Select(issue => $"{issue.Code}: {issue.Message}")));
        }

        await _workspaceService.SaveAsync(workspace, cancellationToken: cancellationToken).ConfigureAwait(false);
        return workspace;
    }

    private static void ValidateDomainRules(Workspace workspace, BusinessDataVaultAuthoringRequest request)
    {
        if (string.Equals(request.EntityName, "BusinessSameAsLink", StringComparison.Ordinal))
        {
            var sameAsLink = workspace.Instance.GetOrCreateEntityRecords("BusinessSameAsLink")
                .Single(record => string.Equals(record.Id, request.RecordId, StringComparison.Ordinal));
            sameAsLink.RelationshipIds.TryGetValue("PrimaryHubId", out var primaryHubId);
            sameAsLink.RelationshipIds.TryGetValue("EquivalentHubId", out var equivalentHubId);

            if (string.Equals(primaryHubId, equivalentHubId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("BusinessSameAsLink requires distinct PrimaryHubId and EquivalentHubId.");
            }
        }

        if (string.Equals(request.EntityName, "BusinessBridgeLink", StringComparison.Ordinal) ||
            string.Equals(request.EntityName, "BusinessBridgeHub", StringComparison.Ordinal))
        {
            ValidateBridgeOrdinalUniqueness(workspace, request.EntityName, request.RecordId);
        }
    }

    private static void ValidateBridgeOrdinalUniqueness(Workspace workspace, string entityName, string recordId)
    {
        var bridgeLinkRecords = workspace.Instance.GetOrCreateEntityRecords("BusinessBridgeLink");
        var bridgeHubRecords = workspace.Instance.GetOrCreateEntityRecords("BusinessBridgeHub");
        var sourceRecords = string.Equals(entityName, "BusinessBridgeLink", StringComparison.Ordinal)
            ? bridgeLinkRecords
            : bridgeHubRecords;

        var record = sourceRecords.Single(row => string.Equals(row.Id, recordId, StringComparison.Ordinal));
        if (!record.RelationshipIds.TryGetValue("BusinessBridgeId", out var bridgeId) ||
            string.IsNullOrWhiteSpace(bridgeId) ||
            !record.Values.TryGetValue("Ordinal", out var ordinal) ||
            string.IsNullOrWhiteSpace(ordinal))
        {
            return;
        }

        var duplicateLink = bridgeLinkRecords.Any(row =>
            !string.Equals(row.Id, recordId, StringComparison.Ordinal) &&
            string.Equals(row.RelationshipIds.GetValueOrDefault("BusinessBridgeId"), bridgeId, StringComparison.Ordinal) &&
            string.Equals(row.Values.GetValueOrDefault("Ordinal"), ordinal, StringComparison.Ordinal));
        var duplicateHub = bridgeHubRecords.Any(row =>
            !string.Equals(row.Id, recordId, StringComparison.Ordinal) &&
            string.Equals(row.RelationshipIds.GetValueOrDefault("BusinessBridgeId"), bridgeId, StringComparison.Ordinal) &&
            string.Equals(row.Values.GetValueOrDefault("Ordinal"), ordinal, StringComparison.Ordinal));

        if (duplicateLink || duplicateHub)
        {
            throw new InvalidOperationException($"Bridge '{bridgeId}' already contains ordinal '{ordinal}'. Bridge path ordinals must be unique.");
        }
    }
}
