using System.Collections;
using System.Reflection;
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
}

public sealed class AnalyticsAuthoringService : IAnalyticsAuthoringService
{
    private const string ModelName = "MetaAnalytics";

    private static readonly Type ModelType = typeof(MetaAnalyticsModel);

    public MetaAnalyticsModel CreateWorkspace() => MetaAnalyticsModel.CreateEmpty();

    public MetaAnalyticsModel AddRecord(AnalyticsAuthoringRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var model = MetaAnalyticsTooling.Load(workspacePath);
        var entityType = ResolveEntityType(request.EntityName);
        var rows = GetEntityRows(model, entityType, request.EntityName);
        if (rows.Cast<object>().Any(row => string.Equals(ReadId(row), request.RecordId, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException($"{request.EntityName} '{request.RecordId}' already exists.");
        }

        var rowToAdd = Activator.CreateInstance(entityType)
            ?? throw new InvalidOperationException($"Could not create {request.EntityName} row.");
        SetText(rowToAdd, "Id", request.RecordId, request.EntityName);

        foreach (var value in request.Values)
        {
            SetText(rowToAdd, value.Key, value.Value, request.EntityName);
        }

        foreach (var relationship in request.Relationships)
        {
            AssignRelationship(model, rowToAdd, request.EntityName, relationship);
        }

        AssignOrdinalIfMissing(model, rowToAdd, request);
        rows.Add(rowToAdd);
        ValidateDomainRules(model, rowToAdd);
        model.SaveToXmlWorkspace(workspacePath);
        return model;
    }

    private static Type ResolveEntityType(string entityName)
    {
        var type = ModelType.Assembly.GetType($"MetaAnalytics.{entityName}", throwOnError: false);
        if (type is null)
        {
            throw new InvalidOperationException($"Entity '{entityName}' was not found in model '{ModelName}'.");
        }

        return type;
    }

    private static IList GetEntityRows(MetaAnalyticsModel model, Type entityType, string entityName)
    {
        var listProperty = ModelType.GetProperty($"{entityName}List", BindingFlags.Instance | BindingFlags.Public);
        if (listProperty is null)
        {
            throw new InvalidOperationException($"Model '{ModelName}' does not expose {entityName}List.");
        }

        if (!listProperty.PropertyType.IsGenericType ||
            listProperty.PropertyType.GetGenericTypeDefinition() != typeof(List<>) ||
            listProperty.PropertyType.GetGenericArguments()[0] != entityType)
        {
            throw new InvalidOperationException($"Model list '{listProperty.Name}' is not List<{entityName}>.");
        }

        return (IList)(listProperty.GetValue(model)
            ?? throw new InvalidOperationException($"Model list '{listProperty.Name}' is null."));
    }

    private static object ResolveRow(MetaAnalyticsModel model, string entityName, string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var targetType = ResolveEntityType(entityName);
        var rows = GetEntityRows(model, targetType, entityName);
        var matches = rows.Cast<object>()
            .Where(row => string.Equals(ReadId(row), id, StringComparison.Ordinal))
            .Take(2)
            .ToArray();

        return matches.Length switch
        {
            1 => matches[0],
            0 => throw new InvalidOperationException($"{entityName} '{id}' was not found."),
            _ => throw new InvalidOperationException($"{entityName} '{id}' matched more than one row.")
        };
    }

    private static void SetText(object row, string propertyName, string value, string entityName)
    {
        var property = row.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException($"Entity '{entityName}' does not define property '{propertyName}'.");
        if (!property.CanWrite || property.PropertyType != typeof(string))
        {
            throw new InvalidOperationException($"Entity '{entityName}' property '{propertyName}' is not a text property.");
        }

        property.SetValue(row, value);
    }

    private static void AssignRelationship(
        MetaAnalyticsModel model,
        object row,
        string entityName,
        AnalyticsRelationshipAssignment relationship)
    {
        var propertyName = RelationshipPropertyName(relationship.ColumnName);
        var property = row.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException($"Entity '{entityName}' does not define relationship '{propertyName}'.");

        var target = ResolveRow(model, relationship.TargetEntityName, relationship.TargetRecordId);
        if (!CanAssignRelationship(property.PropertyType, target.GetType()))
        {
            throw new InvalidOperationException(
                $"Entity '{entityName}' relationship '{propertyName}' cannot reference {relationship.TargetEntityName}.");
        }

        property.SetValue(row, target);
    }

    private static bool CanAssignRelationship(Type propertyType, Type targetType)
    {
        var nullableTarget = Nullable.GetUnderlyingType(propertyType);
        return (nullableTarget ?? propertyType).IsAssignableFrom(targetType);
    }

    private static string RelationshipPropertyName(string columnName) =>
        columnName.EndsWith("Id", StringComparison.Ordinal)
            ? columnName[..^2]
            : columnName;

    private static string ReadId(object row)
    {
        var property = row.GetType().GetProperty("Id", BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException($"Entity '{row.GetType().Name}' does not define Id.");
        return (string?)property.GetValue(row) ?? string.Empty;
    }

    private static void AssignOrdinalIfMissing(
        MetaAnalyticsModel model,
        object rowToAdd,
        AnalyticsAuthoringRequest request)
    {
        var ordinalProperty = rowToAdd.GetType().GetProperty("Ordinal", BindingFlags.Instance | BindingFlags.Public);
        if (ordinalProperty is null || request.Values.ContainsKey("Ordinal"))
        {
            return;
        }

        var rows = GetEntityRows(model, rowToAdd.GetType(), request.EntityName).Cast<object>();
        var ownerRelationship = request.Relationships.FirstOrDefault();
        if (ownerRelationship is not null)
        {
            var ownerPropertyName = RelationshipPropertyName(ownerRelationship.ColumnName);
            var ownerProperty = rowToAdd.GetType().GetProperty(ownerPropertyName, BindingFlags.Instance | BindingFlags.Public)
                ?? throw new InvalidOperationException($"Entity '{request.EntityName}' does not define relationship '{ownerPropertyName}'.");
            var owner = ownerProperty.GetValue(rowToAdd);
            rows = rows.Where(row => ReferenceEquals(ownerProperty.GetValue(row), owner));
        }

        var maxOrdinal = rows
            .Select(row => (string?)ordinalProperty.GetValue(row))
            .Select(value => int.TryParse(value, out var parsed) ? parsed : 0)
            .DefaultIfEmpty(0)
            .Max();

        ordinalProperty.SetValue(rowToAdd, (maxOrdinal + 10).ToString());
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
                break;
            case AggregationBehavior aggregationBehavior:
                ValidateUniqueMeasureAggregationBehavior(model, aggregationBehavior);
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
            case RoleFilter roleFilter:
                ValidateRoleTableScope(roleFilter.SecurityRole, roleFilter.Table, nameof(RoleFilter), roleFilter.Id);
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

    private static void ValidateUniqueMeasureAggregationBehavior(
        MetaAnalyticsModel model,
        AggregationBehavior aggregationBehavior)
    {
        var duplicate = model.AggregationBehaviorList.Any(other =>
            !ReferenceEquals(other, aggregationBehavior) &&
            ReferenceEquals(other.Measure, aggregationBehavior.Measure));
        if (duplicate)
        {
            throw new InvalidOperationException($"Measure '{aggregationBehavior.Measure.Id}' already has an aggregation behavior.");
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
