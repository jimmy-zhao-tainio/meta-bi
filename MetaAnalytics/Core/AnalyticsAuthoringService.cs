using Meta.Integration;
using Meta.Operations;
using Meta.Operations.Domain;
using AnalyticsAttribute = MetaAnalytics.Attribute;
using AnalyticsModel = MetaAnalytics.AnalyticsModel;
using AnalyticsTable = MetaAnalytics.Table;

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
    MetaAnalyticsModel CreateWorkspace();

    MetaAnalyticsModel AddRecord(AnalyticsAuthoringRequest request);

    MetaAnalyticsModel AddMeasure(AnalyticsAuthoringRequest request, string aggregateFunctionKind);
}

public sealed class AnalyticsAuthoringService : IAnalyticsAuthoringService
{
    private const string ModelName = "MetaAnalytics";

    public MetaAnalyticsModel CreateWorkspace() => MetaAnalyticsModel.CreateEmpty();

    public MetaAnalyticsModel AddRecord(AnalyticsAuthoringRequest request) =>
        AddRecordAsync(request).GetAwaiter().GetResult();

    private static async Task<MetaAnalyticsModel> AddRecordAsync(AnalyticsAuthoringRequest request, string? aggregateFunctionKind = null)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);
        await using var workspace = await WorkspaceSurface.OpenAsync(Path.GetFullPath(request.WorkspacePath)).ConfigureAwait(false);
        var state = await WorkspaceComposition.MaterializeAsync(workspace).ConfigureAwait(false);
        var binding = new MetaAnalyticsWorkspaceBinding();
        _ = binding.Read(state);
        var values = new Dictionary<string, string>(request.Values, StringComparer.OrdinalIgnoreCase);
        var relationships = request.Relationships.Select(item => (item.ColumnName, item.TargetEntityName, item.TargetRecordId)).ToList();
        var operations = new List<Operation>();
        if (aggregateFunctionKind is not null)
        {
            var functionId = request.RecordId + ":aggregate-function";
            var variant = aggregateFunctionKind.Trim().ToUpperInvariant() switch
            {
                "SUM" => nameof(SumAggregateFunction),
                "AVERAGE" => nameof(AverageAggregateFunction),
                "COUNT" => nameof(CountAggregateFunction),
                "DISTINCTCOUNT" => nameof(DistinctCountAggregateFunction),
                "MINIMUM" => nameof(MinimumAggregateFunction),
                "MAXIMUM" => nameof(MaximumAggregateFunction),
                _ => throw new InvalidOperationException($"Aggregate function kind '{aggregateFunctionKind}' is not modeled by MetaAnalytics."),
            };
            operations.Add(BuildInsert(state, nameof(AggregateFunction), functionId, new(), []));
            operations.Add(BuildInsert(state, variant, functionId + ":type", new(), [("AggregateFunctionId", nameof(AggregateFunction), functionId)]));
            relationships.RemoveAll(item => item.ColumnName is "AggregateFunction" or "AggregateFunctionId");
            relationships.Add(("AggregateFunctionId", nameof(AggregateFunction), functionId));
        }
        operations.Add(BuildInsert(state, request.EntityName, request.RecordId, values, relationships));
        var candidate = InMemoryOperations.ApplyBatch(state, operations);
        var model = binding.Read(candidate);
        object? added = state.Model.FindEntity(request.EntityName)!.Name switch
        {
            nameof(SortByAttribute) => model.SortByAttributeList.Single(row => row.Id == request.RecordId),
            nameof(AttributeRelationship) => model.AttributeRelationshipList.Single(row => row.Id == request.RecordId),
            nameof(HierarchyLevel) => model.HierarchyLevelList.Single(row => row.Id == request.RecordId),
            nameof(Relationship) => model.RelationshipList.Single(row => row.Id == request.RecordId),
            nameof(Measure) => model.MeasureList.Single(row => row.Id == request.RecordId),
            nameof(PerspectiveTable) => model.PerspectiveTableList.Single(row => row.Id == request.RecordId),
            nameof(PerspectiveAttribute) => model.PerspectiveAttributeList.Single(row => row.Id == request.RecordId),
            nameof(PerspectiveHierarchy) => model.PerspectiveHierarchyList.Single(row => row.Id == request.RecordId),
            nameof(PerspectiveMeasure) => model.PerspectiveMeasureList.Single(row => row.Id == request.RecordId),
            nameof(TablePermission) => model.TablePermissionList.Single(row => row.Id == request.RecordId),
            nameof(AttributePermission) => model.AttributePermissionList.Single(row => row.Id == request.RecordId),
            nameof(TableTranslation) => model.TableTranslationList.Single(row => row.Id == request.RecordId),
            nameof(AttributeTranslation) => model.AttributeTranslationList.Single(row => row.Id == request.RecordId),
            nameof(HierarchyTranslation) => model.HierarchyTranslationList.Single(row => row.Id == request.RecordId),
            nameof(MeasureTranslation) => model.MeasureTranslationList.Single(row => row.Id == request.RecordId),
            nameof(PerspectiveTranslation) => model.PerspectiveTranslationList.Single(row => row.Id == request.RecordId),
            _ => null,
        };
        if (added is not null) ValidateDomainRules(model, added);
        await workspace.ExecuteAsync(operations).ConfigureAwait(false);
        return model;
    }

    public MetaAnalyticsModel AddMeasure(AnalyticsAuthoringRequest request, string aggregateFunctionKind)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(aggregateFunctionKind);
        if (request.EntityName != nameof(Measure))
            throw new InvalidOperationException($"A measure request must target entity '{nameof(Measure)}'.");
        return AddRecordAsync(request, aggregateFunctionKind).GetAwaiter().GetResult();
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

    private static void ValidateDomainRules(MetaAnalyticsModel model, object row)
    {
        switch (row)
        {
            case SortByAttribute sortByAttribute:
                ValidateSameTableAttributes(sortByAttribute.SourceAttribute, sortByAttribute.SortAttribute, nameof(SortByAttribute), sortByAttribute.Id);
                break;
            case AttributeRelationship attributeRelationship:
                ValidateSameTableAttributes(attributeRelationship.ChildAttribute, attributeRelationship.ParentAttribute, nameof(AttributeRelationship), attributeRelationship.Id);
                break;
            case HierarchyLevel hierarchyLevel:
                ValidateHierarchyLevel(model, hierarchyLevel);
                break;
            case Relationship relationship:
                ValidateRelationship(relationship);
                break;
            case Measure measure:
                ValidateTableAttributePair(measure.Table, measure.SourceAttribute, nameof(Measure), measure.Id);
                ValidateAggregateFunction(model, measure);
                break;
            case PerspectiveTable perspectiveTable:
                ValidatePerspectiveItem(perspectiveTable.Perspective, perspectiveTable.Table.AnalyticsModel, nameof(PerspectiveTable), perspectiveTable.Id);
                break;
            case PerspectiveAttribute perspectiveAttribute:
                ValidatePerspectiveItem(perspectiveAttribute.Perspective, perspectiveAttribute.Attribute.Table.AnalyticsModel, nameof(PerspectiveAttribute), perspectiveAttribute.Id);
                break;
            case PerspectiveHierarchy perspectiveHierarchy:
                ValidatePerspectiveItem(perspectiveHierarchy.Perspective, perspectiveHierarchy.Hierarchy.Table.AnalyticsModel, nameof(PerspectiveHierarchy), perspectiveHierarchy.Id);
                break;
            case PerspectiveMeasure perspectiveMeasure:
                ValidatePerspectiveItem(perspectiveMeasure.Perspective, perspectiveMeasure.Measure.Table.AnalyticsModel, nameof(PerspectiveMeasure), perspectiveMeasure.Id);
                break;
            case TablePermission tablePermission:
                ValidateRoleTableScope(tablePermission.SecurityRole, tablePermission.Table, nameof(TablePermission), tablePermission.Id);
                break;
            case AttributePermission attributePermission:
                ValidateRoleTableScope(attributePermission.SecurityRole, attributePermission.Attribute.Table, nameof(AttributePermission), attributePermission.Id);
                break;
            case TableTranslation tableTranslation:
                ValidateTranslationScope(tableTranslation.Culture, tableTranslation.Table.AnalyticsModel, nameof(TableTranslation), tableTranslation.Id);
                break;
            case AttributeTranslation attributeTranslation:
                ValidateTranslationScope(attributeTranslation.Culture, attributeTranslation.Attribute.Table.AnalyticsModel, nameof(AttributeTranslation), attributeTranslation.Id);
                break;
            case HierarchyTranslation hierarchyTranslation:
                ValidateTranslationScope(hierarchyTranslation.Culture, hierarchyTranslation.Hierarchy.Table.AnalyticsModel, nameof(HierarchyTranslation), hierarchyTranslation.Id);
                break;
            case MeasureTranslation measureTranslation:
                ValidateTranslationScope(measureTranslation.Culture, measureTranslation.Measure.Table.AnalyticsModel, nameof(MeasureTranslation), measureTranslation.Id);
                break;
            case PerspectiveTranslation perspectiveTranslation:
                ValidateTranslationScope(perspectiveTranslation.Culture, perspectiveTranslation.Perspective.AnalyticsModel, nameof(PerspectiveTranslation), perspectiveTranslation.Id);
                break;
        }
    }

    private static void ValidateHierarchyLevel(MetaAnalyticsModel model, HierarchyLevel level)
    {
        if (!ReferenceEquals(level.Hierarchy.Table, level.Attribute.Table))
        {
            throw new InvalidOperationException($"HierarchyLevel '{level.Id}' references attribute '{level.Attribute.Id}' outside hierarchy table.");
        }

        var duplicate = model.HierarchyLevelList.Any(other =>
            !ReferenceEquals(other, level) &&
            ReferenceEquals(other.Hierarchy, level.Hierarchy) &&
            string.Equals(other.Ordinal, level.Ordinal, StringComparison.Ordinal));
        if (duplicate)
        {
            throw new InvalidOperationException($"HierarchyLevel owner '{level.Hierarchy.Id}' already contains ordinal '{level.Ordinal}'.");
        }
    }

    private static void ValidateSameTableAttributes(
        AnalyticsAttribute left,
        AnalyticsAttribute right,
        string entityName,
        string id)
    {
        if (!ReferenceEquals(left.Table, right.Table))
        {
            throw new InvalidOperationException($"{entityName} '{id}' references attributes from different tables.");
        }
    }

    private static void ValidateRelationship(Relationship relationship)
    {
        ValidateTableAttributePair(relationship.FromTable, relationship.FromAttribute, nameof(Relationship), relationship.Id);
        ValidateTableAttributePair(relationship.ToTable, relationship.ToAttribute, nameof(Relationship), relationship.Id);
        if (relationship.GranularityAttribute is not null &&
            !ReferenceEquals(relationship.GranularityAttribute.Table, relationship.ToTable))
        {
            throw new InvalidOperationException($"{nameof(Relationship)} '{relationship.Id}' references optional attribute '{relationship.GranularityAttribute.Id}' outside table '{relationship.ToTable.Id}'.");
        }
    }

    private static void ValidateTableAttributePair(
        AnalyticsTable table,
        AnalyticsAttribute attribute,
        string entityName,
        string id)
    {
        if (!ReferenceEquals(attribute.Table, table))
        {
            throw new InvalidOperationException($"{entityName} '{id}' references attribute '{attribute.Id}' outside table '{table.Id}'.");
        }
    }

    private static void ValidateAggregateFunction(MetaAnalyticsModel model, Measure measure)
    {
        var variantCount = model.SumAggregateFunctionList.Count(row => ReferenceEquals(row.AggregateFunction, measure.AggregateFunction))
            + model.AverageAggregateFunctionList.Count(row => ReferenceEquals(row.AggregateFunction, measure.AggregateFunction))
            + model.CountAggregateFunctionList.Count(row => ReferenceEquals(row.AggregateFunction, measure.AggregateFunction))
            + model.DistinctCountAggregateFunctionList.Count(row => ReferenceEquals(row.AggregateFunction, measure.AggregateFunction))
            + model.MinimumAggregateFunctionList.Count(row => ReferenceEquals(row.AggregateFunction, measure.AggregateFunction))
            + model.MaximumAggregateFunctionList.Count(row => ReferenceEquals(row.AggregateFunction, measure.AggregateFunction));
        if (variantCount != 1)
        {
            throw new InvalidOperationException(
                $"Measure '{measure.Id}' must reference one concrete aggregate-function entity; found {variantCount}.");
        }
    }

    private static void ValidatePerspectiveItem(
        Perspective perspective,
        AnalyticsModel targetModel,
        string entityName,
        string id)
    {
        if (!ReferenceEquals(perspective.AnalyticsModel, targetModel))
        {
            throw new InvalidOperationException($"{entityName} '{id}' references an item outside perspective model '{perspective.AnalyticsModel.Id}'.");
        }
    }

    private static void ValidateRoleTableScope(
        SecurityRole role,
        AnalyticsTable table,
        string entityName,
        string id)
    {
        if (!ReferenceEquals(role.AnalyticsModel, table.AnalyticsModel))
        {
            throw new InvalidOperationException($"{entityName} '{id}' references table outside role model.");
        }
    }

    private static void ValidateTranslationScope(
        Culture culture,
        AnalyticsModel targetModel,
        string entityName,
        string id)
    {
        if (!ReferenceEquals(culture.AnalyticsModel, targetModel))
        {
            throw new InvalidOperationException($"{entityName} '{id}' references an item outside culture model '{culture.AnalyticsModel.Id}'.");
        }
    }

}
