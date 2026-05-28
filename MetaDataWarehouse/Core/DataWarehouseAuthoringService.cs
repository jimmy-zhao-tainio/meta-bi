using Meta.Core.Domain;
using Meta.Core.Services;

namespace MetaDataWarehouse.Core;

public sealed class DataWarehouseAuthoringRequest
{
    public required string WorkspacePath { get; init; }
    public required string EntityName { get; init; }
    public required string RecordId { get; init; }
    public Dictionary<string, string> Values { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<DataWarehouseRelationshipAssignment> Relationships { get; } = new();
}

public sealed record DataWarehouseRelationshipAssignment(
    string ColumnName,
    string TargetEntityName,
    string TargetRecordId);

public interface IDataWarehouseAuthoringService
{
    Task<Workspace> AddRecordAsync(DataWarehouseAuthoringRequest request, CancellationToken cancellationToken = default);
}

public sealed class DataWarehouseAuthoringService : IDataWarehouseAuthoringService
{
    private readonly IWorkspaceService workspaceService;
    private readonly ValidationService validationService;

    public DataWarehouseAuthoringService()
        : this(new WorkspaceService(), new ValidationService())
    {
    }

    public DataWarehouseAuthoringService(IWorkspaceService workspaceService, ValidationService validationService)
    {
        this.workspaceService = workspaceService ?? throw new ArgumentNullException(nameof(workspaceService));
        this.validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    }

    public async Task<Workspace> AddRecordAsync(
        DataWarehouseAuthoringRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var workspace = await workspaceService.LoadAsync(workspacePath, searchUpward: false, cancellationToken)
            .ConfigureAwait(false);
        if (!string.Equals(workspace.Model.Name, MetaDataWarehouseModels.MetaDataWarehouseModelName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Workspace '{workspacePath}' contained model '{workspace.Model.Name}', not '{MetaDataWarehouseModels.MetaDataWarehouseModelName}'.");
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
        ValidateDomainRules(workspace, request);

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
        IReadOnlyList<DataWarehouseRelationshipAssignment> relationships)
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

    private static void ValidateDomainRules(Workspace workspace, DataWarehouseAuthoringRequest request)
    {
        if (string.Equals(request.EntityName, "FactDimension", StringComparison.Ordinal))
        {
            ValidateUniqueRoleWithinOwner(workspace, "FactDimension", request.RecordId, "FactId", "RoleName");
        }

        if (string.Equals(request.EntityName, "DimensionBusinessKeyPart", StringComparison.Ordinal))
        {
            ValidateRelatedDimensionAttribute(
                workspace,
                "DimensionBusinessKeyPart",
                request.RecordId,
                ownerRelationshipColumnName: "DimensionBusinessKeyId",
                ownerEntityName: "DimensionBusinessKey",
                attributeRelationshipColumnName: "DimensionAttributeId",
                ownerDimensionRelationshipColumnName: "DimensionId");
        }

        if (string.Equals(request.EntityName, "Type1DimensionAttribute", StringComparison.Ordinal) ||
            string.Equals(request.EntityName, "Type2DimensionAttribute", StringComparison.Ordinal))
        {
            ValidateRelatedDimensionAttribute(
                workspace,
                request.EntityName,
                request.RecordId,
                ownerRelationshipColumnName: "SlowlyChangingDimensionId",
                ownerEntityName: "SlowlyChangingDimension",
                attributeRelationshipColumnName: "DimensionAttributeId",
                ownerDimensionRelationshipColumnName: "DimensionId");
        }

        if (string.Equals(request.EntityName, "DimensionHierarchyLevel", StringComparison.Ordinal))
        {
            ValidateRelatedDimensionAttribute(
                workspace,
                "DimensionHierarchyLevel",
                request.RecordId,
                ownerRelationshipColumnName: "DimensionHierarchyId",
                ownerEntityName: "DimensionHierarchy",
                attributeRelationshipColumnName: "DimensionAttributeId",
                ownerDimensionRelationshipColumnName: "DimensionId");
        }

        if (string.Equals(request.EntityName, "JunkDimensionComponent", StringComparison.Ordinal))
        {
            ValidateRelatedDimensionAttribute(
                workspace,
                "JunkDimensionComponent",
                request.RecordId,
                ownerRelationshipColumnName: "JunkDimensionId",
                ownerEntityName: "JunkDimension",
                attributeRelationshipColumnName: "DimensionAttributeId",
                ownerDimensionRelationshipColumnName: "DimensionId");
        }

        if (string.Equals(request.EntityName, "FactBridge", StringComparison.Ordinal))
        {
            ValidateUniqueRoleWithinOwner(workspace, "FactBridge", request.RecordId, "FactId", "RoleName");
        }

        if (string.Equals(request.EntityName, "BridgeParticipant", StringComparison.Ordinal))
        {
            ValidateUniqueRoleWithinOwner(workspace, "BridgeParticipant", request.RecordId, "BridgeTableId", "RoleName");
            ValidateUniqueOrdinalWithinOwner(workspace, "BridgeParticipant", request.RecordId, "BridgeTableId");
        }

        if (string.Equals(request.EntityName, "AggregateFact", StringComparison.Ordinal))
        {
            var row = workspace.Instance.GetOrCreateEntityRecords("AggregateFact")
                .Single(record => string.Equals(record.Id, request.RecordId, StringComparison.Ordinal));
            row.RelationshipIds.TryGetValue("AggregatedFactId", out var aggregateFactId);
            row.RelationshipIds.TryGetValue("SourceFactId", out var sourceFactId);
            if (string.Equals(aggregateFactId, sourceFactId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("AggregateFact requires distinct AggregatedFactId and SourceFactId.");
            }
        }
    }

    private static void ValidateRelatedDimensionAttribute(
        Workspace workspace,
        string entityName,
        string recordId,
        string ownerRelationshipColumnName,
        string ownerEntityName,
        string attributeRelationshipColumnName,
        string ownerDimensionRelationshipColumnName)
    {
        var row = workspace.Instance.GetOrCreateEntityRecords(entityName)
            .Single(record => string.Equals(record.Id, recordId, StringComparison.Ordinal));
        row.RelationshipIds.TryGetValue(ownerRelationshipColumnName, out var ownerId);
        row.RelationshipIds.TryGetValue(attributeRelationshipColumnName, out var attributeId);
        if (string.IsNullOrWhiteSpace(ownerId) || string.IsNullOrWhiteSpace(attributeId))
        {
            return;
        }

        var owner = workspace.Instance.GetOrCreateEntityRecords(ownerEntityName)
            .Single(record => string.Equals(record.Id, ownerId, StringComparison.Ordinal));
        var attribute = workspace.Instance.GetOrCreateEntityRecords("DimensionAttribute")
            .Single(record => string.Equals(record.Id, attributeId, StringComparison.Ordinal));

        owner.RelationshipIds.TryGetValue(ownerDimensionRelationshipColumnName, out var ownerDimensionId);
        attribute.RelationshipIds.TryGetValue("DimensionId", out var attributeDimensionId);
        if (!string.Equals(ownerDimensionId, attributeDimensionId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"{entityName} '{recordId}' references attribute '{attributeId}' from dimension '{attributeDimensionId}', but '{ownerEntityName}' '{ownerId}' belongs to dimension '{ownerDimensionId}'.");
        }
    }

    private static void ValidateUniqueRoleWithinOwner(
        Workspace workspace,
        string entityName,
        string recordId,
        string ownerRelationshipColumnName,
        string rolePropertyName)
    {
        var row = workspace.Instance.GetOrCreateEntityRecords(entityName)
            .Single(record => string.Equals(record.Id, recordId, StringComparison.Ordinal));
        if (!row.RelationshipIds.TryGetValue(ownerRelationshipColumnName, out var ownerId) ||
            string.IsNullOrWhiteSpace(ownerId) ||
            !row.Values.TryGetValue(rolePropertyName, out var roleName) ||
            string.IsNullOrWhiteSpace(roleName))
        {
            return;
        }

        var duplicate = workspace.Instance.GetOrCreateEntityRecords(entityName).Any(other =>
            !string.Equals(other.Id, recordId, StringComparison.Ordinal) &&
            string.Equals(other.RelationshipIds.GetValueOrDefault(ownerRelationshipColumnName), ownerId, StringComparison.Ordinal) &&
            string.Equals(other.Values.GetValueOrDefault(rolePropertyName), roleName, StringComparison.OrdinalIgnoreCase));

        if (duplicate)
        {
            throw new InvalidOperationException($"{entityName} owner '{ownerId}' already contains role '{roleName}'.");
        }
    }

    private static void ValidateUniqueOrdinalWithinOwner(
        Workspace workspace,
        string entityName,
        string recordId,
        string ownerRelationshipColumnName)
    {
        var row = workspace.Instance.GetOrCreateEntityRecords(entityName)
            .Single(record => string.Equals(record.Id, recordId, StringComparison.Ordinal));
        if (!row.RelationshipIds.TryGetValue(ownerRelationshipColumnName, out var ownerId) ||
            string.IsNullOrWhiteSpace(ownerId) ||
            !row.Values.TryGetValue("Ordinal", out var ordinal) ||
            string.IsNullOrWhiteSpace(ordinal))
        {
            return;
        }

        var duplicate = workspace.Instance.GetOrCreateEntityRecords(entityName).Any(other =>
            !string.Equals(other.Id, recordId, StringComparison.Ordinal) &&
            string.Equals(other.RelationshipIds.GetValueOrDefault(ownerRelationshipColumnName), ownerId, StringComparison.Ordinal) &&
            string.Equals(other.Values.GetValueOrDefault("Ordinal"), ordinal, StringComparison.Ordinal));

        if (duplicate)
        {
            throw new InvalidOperationException($"{entityName} owner '{ownerId}' already contains ordinal '{ordinal}'.");
        }
    }
}
