using System.Collections;
using System.Reflection;
using MetaRawDataVault;

namespace MetaDataVault.Core;

public sealed class RawDataVaultAuthoringRequest
{
    public required string WorkspacePath { get; init; }
    public required string EntityName { get; init; }
    public required string RecordId { get; init; }
    public Dictionary<string, string> Values { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<RawDataVaultRelationshipAssignment> Relationships { get; } = new();
}

public sealed record RawDataVaultRelationshipAssignment(string ColumnName, string TargetEntityName, string TargetRecordId);

public interface IRawDataVaultAuthoringService
{
    MetaRawDataVaultModel CreateWorkspace();

    MetaRawDataVaultModel AddRecord(RawDataVaultAuthoringRequest request);
}

public sealed class RawDataVaultAuthoringService : IRawDataVaultAuthoringService
{
    private const string ModelName = "MetaRawDataVault";

    private static readonly Type ModelType = typeof(MetaRawDataVaultModel);

    public MetaRawDataVaultModel CreateWorkspace() => MetaRawDataVaultModel.CreateEmpty();

    public MetaRawDataVaultModel AddRecord(RawDataVaultAuthoringRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var model = Meta.Core.Serialization.TypedWorkspaceModelMapper.Load<MetaRawDataVaultModel>(workspacePath);
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

        rows.Add(rowToAdd);
        Meta.Core.Serialization.TypedWorkspaceModelMapper.Save(model, workspacePath);
        return model;
    }

    private static Type ResolveEntityType(string entityName)
    {
        var type = ModelType.Assembly.GetType($"MetaRawDataVault.{entityName}", throwOnError: false);
        if (type is null)
        {
            throw new InvalidOperationException($"Entity '{entityName}' was not found in model '{ModelName}'.");
        }

        return type;
    }

    private static IList GetEntityRows(MetaRawDataVaultModel model, Type entityType, string entityName)
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

    private static object ResolveRow(MetaRawDataVaultModel model, string entityName, string id)
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
        MetaRawDataVaultModel model,
        object row,
        string entityName,
        RawDataVaultRelationshipAssignment relationship)
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

}
