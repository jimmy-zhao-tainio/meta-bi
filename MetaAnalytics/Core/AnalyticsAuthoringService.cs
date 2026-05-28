using Meta.Core.Domain;
using Meta.Core.Services;

namespace MetaAnalytics.Core;

public sealed class AnalyticsAuthoringRequest
{
    public required string WorkspacePath { get; init; }
    public required string EntityName { get; init; }
    public required string RecordId { get; init; }
    public Dictionary<string, string> Values { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<AnalyticsRelationshipAssignment> Relationships { get; } = new();
}

public sealed record AnalyticsRelationshipAssignment(
    string ColumnName,
    string TargetEntityName,
    string TargetRecordId);

public interface IAnalyticsAuthoringService
{
    Task<Workspace> AddRecordAsync(AnalyticsAuthoringRequest request, CancellationToken cancellationToken = default);
}

public sealed class AnalyticsAuthoringService : IAnalyticsAuthoringService
{
    private readonly IWorkspaceService workspaceService;
    private readonly ValidationService validationService;

    public AnalyticsAuthoringService()
        : this(new WorkspaceService(), new ValidationService())
    {
    }

    public AnalyticsAuthoringService(IWorkspaceService workspaceService, ValidationService validationService)
    {
        this.workspaceService = workspaceService ?? throw new ArgumentNullException(nameof(workspaceService));
        this.validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
    }

    public async Task<Workspace> AddRecordAsync(
        AnalyticsAuthoringRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var workspace = await workspaceService.LoadAsync(workspacePath, searchUpward: false, cancellationToken)
            .ConfigureAwait(false);
        if (!string.Equals(workspace.Model.Name, MetaAnalyticsModels.MetaAnalyticsModelName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Workspace '{workspacePath}' contained model '{workspace.Model.Name}', not '{MetaAnalyticsModels.MetaAnalyticsModelName}'.");
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
        ValidateDomainRules(workspace, request.EntityName, request.RecordId);

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
        IReadOnlyList<AnalyticsRelationshipAssignment> relationships)
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

    private static void ValidateDomainRules(Workspace workspace, string entityName, string recordId)
    {
        if (string.Equals(entityName, "SortByAttribute", StringComparison.Ordinal))
        {
            ValidateSameTableAttributes(workspace, entityName, recordId, "SourceAttributeId", "SortAttributeId");
        }

        if (string.Equals(entityName, "AttributeRelationship", StringComparison.Ordinal))
        {
            ValidateSameTableAttributes(workspace, entityName, recordId, "ChildAttributeId", "ParentAttributeId");
        }

        if (string.Equals(entityName, "HierarchyLevel", StringComparison.Ordinal))
        {
            ValidateHierarchyLevel(workspace, recordId);
            ValidateUniqueOrdinalWithinOwner(workspace, entityName, recordId, "HierarchyId");
        }

        if (string.Equals(entityName, "Relationship", StringComparison.Ordinal))
        {
            ValidateTableAttributePair(workspace, entityName, recordId, "FromTableId", "FromAttributeId");
            ValidateTableAttributePair(workspace, entityName, recordId, "ToTableId", "ToAttributeId");
            ValidateOptionalTableAttributePair(workspace, entityName, recordId, "ToTableId", "GranularityAttributeId");
        }

        if (string.Equals(entityName, "Measure", StringComparison.Ordinal))
        {
            ValidateTableAttributePair(workspace, entityName, recordId, "TableId", "SourceAttributeId");
        }

        if (string.Equals(entityName, "AggregationBehavior", StringComparison.Ordinal))
        {
            ValidateUniqueMeasureAggregationBehavior(workspace, recordId);
        }

        if (entityName.StartsWith("Perspective", StringComparison.Ordinal) &&
            !string.Equals(entityName, "Perspective", StringComparison.Ordinal))
        {
            ValidatePerspectiveItem(workspace, entityName, recordId);
        }

        if (string.Equals(entityName, "RoleFilter", StringComparison.Ordinal) ||
            string.Equals(entityName, "TablePermission", StringComparison.Ordinal))
        {
            ValidateRoleTableScope(workspace, entityName, recordId, "TableId");
        }

        if (string.Equals(entityName, "AttributePermission", StringComparison.Ordinal))
        {
            ValidateRoleAttributeScope(workspace, recordId);
        }

        if (entityName.EndsWith("Translation", StringComparison.Ordinal))
        {
            ValidateTranslationScope(workspace, entityName, recordId);
        }
    }

    private static void ValidateHierarchyLevel(Workspace workspace, string recordId)
    {
        var row = GetRecord(workspace, "HierarchyLevel", recordId);
        var hierarchyId = row.RelationshipIds.GetValueOrDefault("HierarchyId");
        var attributeId = row.RelationshipIds.GetValueOrDefault("AttributeId");
        var hierarchy = GetRecord(workspace, "Hierarchy", hierarchyId!);
        var attribute = GetRecord(workspace, "Attribute", attributeId!);
        if (!string.Equals(hierarchy.RelationshipIds.GetValueOrDefault("TableId"), attribute.RelationshipIds.GetValueOrDefault("TableId"), StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"HierarchyLevel '{recordId}' references attribute '{attributeId}' outside hierarchy table.");
        }
    }

    private static void ValidateSameTableAttributes(Workspace workspace, string entityName, string recordId, string leftColumnName, string rightColumnName)
    {
        var row = GetRecord(workspace, entityName, recordId);
        var left = GetRecord(workspace, "Attribute", row.RelationshipIds.GetValueOrDefault(leftColumnName)!);
        var right = GetRecord(workspace, "Attribute", row.RelationshipIds.GetValueOrDefault(rightColumnName)!);
        if (!string.Equals(left.RelationshipIds.GetValueOrDefault("TableId"), right.RelationshipIds.GetValueOrDefault("TableId"), StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"{entityName} '{recordId}' references attributes from different tables.");
        }
    }

    private static void ValidateTableAttributePair(Workspace workspace, string entityName, string recordId, string tableColumnName, string attributeColumnName)
    {
        var row = GetRecord(workspace, entityName, recordId);
        var tableId = row.RelationshipIds.GetValueOrDefault(tableColumnName);
        var attribute = GetRecord(workspace, "Attribute", row.RelationshipIds.GetValueOrDefault(attributeColumnName)!);
        if (!string.Equals(attribute.RelationshipIds.GetValueOrDefault("TableId"), tableId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"{entityName} '{recordId}' references attribute '{attribute.Id}' outside table '{tableId}'.");
        }
    }

    private static void ValidateOptionalTableAttributePair(Workspace workspace, string entityName, string recordId, string tableColumnName, string attributeColumnName)
    {
        var row = GetRecord(workspace, entityName, recordId);
        if (!row.RelationshipIds.TryGetValue(attributeColumnName, out var attributeId) ||
            string.IsNullOrWhiteSpace(attributeId))
        {
            return;
        }

        var tableId = row.RelationshipIds.GetValueOrDefault(tableColumnName);
        var attribute = GetRecord(workspace, "Attribute", attributeId);
        if (!string.Equals(attribute.RelationshipIds.GetValueOrDefault("TableId"), tableId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"{entityName} '{recordId}' references optional attribute '{attributeId}' outside table '{tableId}'.");
        }
    }

    private static void ValidateUniqueMeasureAggregationBehavior(Workspace workspace, string recordId)
    {
        var row = GetRecord(workspace, "AggregationBehavior", recordId);
        var measureId = row.RelationshipIds.GetValueOrDefault("MeasureId");
        if (string.IsNullOrWhiteSpace(measureId))
        {
            return;
        }

        var duplicate = workspace.Instance.GetOrCreateEntityRecords("AggregationBehavior").Any(other =>
            !string.Equals(other.Id, recordId, StringComparison.Ordinal) &&
            string.Equals(other.RelationshipIds.GetValueOrDefault("MeasureId"), measureId, StringComparison.Ordinal));
        if (duplicate)
        {
            throw new InvalidOperationException($"Measure '{measureId}' already has an aggregation behavior.");
        }
    }

    private static void ValidatePerspectiveItem(Workspace workspace, string entityName, string recordId)
    {
        var row = GetRecord(workspace, entityName, recordId);
        var perspective = GetRecord(workspace, "Perspective", row.RelationshipIds.GetValueOrDefault("PerspectiveId")!);
        var modelId = perspective.RelationshipIds.GetValueOrDefault("AnalyticsModelId");
        var targetColumnName = row.RelationshipIds.Keys.Single(key => !string.Equals(key, "PerspectiveId", StringComparison.Ordinal));
        var targetEntityName = targetColumnName[..^2];
        var target = GetRecord(workspace, targetEntityName, row.RelationshipIds[targetColumnName]);
        if (!string.Equals(modelId, GetOwningModelId(workspace, targetEntityName, target), StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"{entityName} '{recordId}' references an item outside perspective model '{modelId}'.");
        }
    }

    private static void ValidateRoleTableScope(Workspace workspace, string entityName, string recordId, string tableColumnName)
    {
        var row = GetRecord(workspace, entityName, recordId);
        var role = GetRecord(workspace, "SecurityRole", row.RelationshipIds.GetValueOrDefault("SecurityRoleId")!);
        var table = GetRecord(workspace, "Table", row.RelationshipIds.GetValueOrDefault(tableColumnName)!);
        if (!string.Equals(role.RelationshipIds.GetValueOrDefault("AnalyticsModelId"), table.RelationshipIds.GetValueOrDefault("AnalyticsModelId"), StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"{entityName} '{recordId}' references table outside role model.");
        }
    }

    private static void ValidateRoleAttributeScope(Workspace workspace, string recordId)
    {
        var row = GetRecord(workspace, "AttributePermission", recordId);
        var role = GetRecord(workspace, "SecurityRole", row.RelationshipIds.GetValueOrDefault("SecurityRoleId")!);
        var attribute = GetRecord(workspace, "Attribute", row.RelationshipIds.GetValueOrDefault("AttributeId")!);
        var table = GetRecord(workspace, "Table", attribute.RelationshipIds.GetValueOrDefault("TableId")!);
        if (!string.Equals(role.RelationshipIds.GetValueOrDefault("AnalyticsModelId"), table.RelationshipIds.GetValueOrDefault("AnalyticsModelId"), StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"AttributePermission '{recordId}' references an attribute outside role model.");
        }
    }

    private static void ValidateTranslationScope(Workspace workspace, string entityName, string recordId)
    {
        var row = GetRecord(workspace, entityName, recordId);
        var culture = GetRecord(workspace, "Culture", row.RelationshipIds.GetValueOrDefault("CultureId")!);
        var modelId = culture.RelationshipIds.GetValueOrDefault("AnalyticsModelId");
        var targetColumnName = row.RelationshipIds.Keys.Single(key => !string.Equals(key, "CultureId", StringComparison.Ordinal));
        var targetEntityName = targetColumnName[..^2];
        var target = GetRecord(workspace, targetEntityName, row.RelationshipIds[targetColumnName]);
        if (!string.Equals(modelId, GetOwningModelId(workspace, targetEntityName, target), StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"{entityName} '{recordId}' references an item outside culture model '{modelId}'.");
        }
    }

    private static string? GetOwningModelId(Workspace workspace, string entityName, GenericRecord row)
    {
        if (row.RelationshipIds.TryGetValue("AnalyticsModelId", out var modelId))
        {
            return modelId;
        }

        if (entityName is "Attribute" or "Hierarchy" or "Measure")
        {
            var table = GetRecord(workspace, "Table", row.RelationshipIds.GetValueOrDefault("TableId")!);
            return table.RelationshipIds.GetValueOrDefault("AnalyticsModelId");
        }

        if (entityName is "HierarchyLevel")
        {
            var hierarchy = GetRecord(workspace, "Hierarchy", row.RelationshipIds.GetValueOrDefault("HierarchyId")!);
            return GetOwningModelId(workspace, "Hierarchy", hierarchy);
        }

        return null;
    }

    private static void ValidateUniqueOrdinalWithinOwner(
        Workspace workspace,
        string entityName,
        string recordId,
        string ownerRelationshipColumnName)
    {
        var row = GetRecord(workspace, entityName, recordId);
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

    private static GenericRecord GetRecord(Workspace workspace, string entityName, string id)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName)
            .Single(record => string.Equals(record.Id, id, StringComparison.Ordinal));
    }
}
