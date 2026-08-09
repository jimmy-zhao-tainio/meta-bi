using System.Collections;
using System.Reflection;

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
    MetaTabularModel CreateWorkspace();

    MetaTabularModel AddRecord(TabularAuthoringRequest request);
}

public sealed class TabularAuthoringService : ITabularAuthoringService
{
    private const string ModelName = "MetaTabular";

    private static readonly Type ModelType = typeof(MetaTabularModel);

    public MetaTabularModel CreateWorkspace() => MetaTabularModel.CreateEmpty();

    public MetaTabularModel AddRecord(TabularAuthoringRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var model = Meta.Core.Serialization.TypedWorkspaceModelMapper.Load<MetaTabularModel>(workspacePath);
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
        Meta.Core.Serialization.TypedWorkspaceModelMapper.Save(model, workspacePath);
        return model;
    }

    private static Type ResolveEntityType(string entityName)
    {
        var type = ModelType.Assembly.GetType($"MetaTabular.{entityName}", throwOnError: false);
        if (type is null)
        {
            throw new InvalidOperationException($"Entity '{entityName}' was not found in model '{ModelName}'.");
        }

        return type;
    }

    private static IList GetEntityRows(MetaTabularModel model, Type entityType, string entityName)
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

    private static object ResolveRow(MetaTabularModel model, string entityName, string id)
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
        MetaTabularModel model,
        object row,
        string entityName,
        TabularRelationshipAssignment relationship)
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
        MetaTabularModel model,
        object rowToAdd,
        TabularAuthoringRequest request)
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
}
