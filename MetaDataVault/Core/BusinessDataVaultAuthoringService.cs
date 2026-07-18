using System.Collections;
using System.Reflection;
using MetaBusinessDataVault;

namespace MetaDataVault.Core;

public sealed class BusinessDataVaultAuthoringRequest
{
    public required string WorkspacePath { get; init; }
    public required string EntityName { get; init; }
    public required string RecordId { get; init; }
    public Dictionary<string, string> Values { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<BusinessDataVaultRelationshipAssignment> Relationships { get; } = new();
    public Dictionary<string, string> DataTypeDetails { get; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed record BusinessDataVaultRelationshipAssignment(string ColumnName, string TargetEntityName, string TargetRecordId);

public sealed record BusinessDataVaultWorkspaceCreationResult(
    string WorkspacePath,
    string ModelName,
    int RowCount);

public interface IBusinessDataVaultAuthoringService
{
    BusinessDataVaultWorkspaceCreationResult CreateWorkspace(string workspacePath);

    MetaBusinessDataVaultModel AddRecord(BusinessDataVaultAuthoringRequest request);
}

public sealed class BusinessDataVaultAuthoringService : IBusinessDataVaultAuthoringService
{
    private const string ModelName = "MetaBusinessDataVault";

    private static readonly Type ModelType = typeof(MetaBusinessDataVaultModel);

    private static readonly IReadOnlyDictionary<string, OrdinalScope> OrdinalScopes =
        new Dictionary<string, OrdinalScope>(StringComparer.Ordinal)
        {
            ["BusinessHubKeyPart"] = new("BusinessHub", ["BusinessHubKeyPart"]),
            ["BusinessReferenceKeyPart"] = new("BusinessReference", ["BusinessReferenceKeyPart"]),
            ["BusinessHubSatelliteAttribute"] = new("BusinessHubSatellite", ["BusinessHubSatelliteAttribute"]),
            ["BusinessLinkSatelliteAttribute"] = new("BusinessLinkSatellite", ["BusinessLinkSatelliteAttribute"]),
            ["BusinessSameAsLinkSatelliteAttribute"] = new("BusinessSameAsLinkSatellite", ["BusinessSameAsLinkSatelliteAttribute"]),
            ["BusinessHierarchicalLinkSatelliteAttribute"] = new("BusinessHierarchicalLinkSatellite", ["BusinessHierarchicalLinkSatelliteAttribute"]),
            ["BusinessReferenceSatelliteAttribute"] = new("BusinessReferenceSatellite", ["BusinessReferenceSatelliteAttribute"]),
            ["BusinessPointInTimeStamp"] = new("BusinessPointInTime", ["BusinessPointInTimeStamp"]),
            ["BusinessPointInTimeHubSatellite"] = new("BusinessPointInTime", ["BusinessPointInTimeHubSatellite", "BusinessPointInTimeLinkSatellite"]),
            ["BusinessPointInTimeLinkSatellite"] = new("BusinessPointInTime", ["BusinessPointInTimeHubSatellite", "BusinessPointInTimeLinkSatellite"]),
        };

    public BusinessDataVaultWorkspaceCreationResult CreateWorkspace(string workspacePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);

        var fullPath = MetaBusinessDataVaultTooling.CreateWorkspace(Path.GetFullPath(workspacePath));
        return new BusinessDataVaultWorkspaceCreationResult(fullPath, ModelName, RowCount: 0);
    }

    public MetaBusinessDataVaultModel AddRecord(BusinessDataVaultAuthoringRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var model = MetaBusinessDataVaultTooling.Load(workspacePath);
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
        AddDataTypeDetails(model, rowToAdd, request);
        ValidateDomainRules(model, rowToAdd, request);

        model.SaveToXmlWorkspace(workspacePath);
        return model;
    }

    private static Type ResolveEntityType(string entityName)
    {
        var type = ModelType.Assembly.GetType($"MetaBusinessDataVault.{entityName}", throwOnError: false);
        if (type is null)
        {
            throw new InvalidOperationException($"Entity '{entityName}' was not found in model '{ModelName}'.");
        }

        return type;
    }

    private static IList GetEntityRows(MetaBusinessDataVaultModel model, Type entityType, string entityName)
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

    private static object ResolveRow(MetaBusinessDataVaultModel model, string entityName, string id)
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
        MetaBusinessDataVaultModel model,
        object row,
        string entityName,
        BusinessDataVaultRelationshipAssignment relationship)
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
        MetaBusinessDataVaultModel model,
        object rowToAdd,
        BusinessDataVaultAuthoringRequest request)
    {
        var ordinalProperty = rowToAdd.GetType().GetProperty("Ordinal", BindingFlags.Instance | BindingFlags.Public);
        if (ordinalProperty is null ||
            request.Values.ContainsKey("Ordinal") ||
            !OrdinalScopes.TryGetValue(request.EntityName, out var scope))
        {
            return;
        }

        var ownerProperty = rowToAdd.GetType().GetProperty(scope.RelationshipPropertyName, BindingFlags.Instance | BindingFlags.Public);
        if (ownerProperty is null)
        {
            return;
        }

        var owner = ownerProperty.GetValue(rowToAdd);
        if (owner is null)
        {
            return;
        }

        var nextOrdinal = scope.EntityNames
            .SelectMany(entityName => GetEntityRows(model, ResolveEntityType(entityName), entityName).Cast<object>())
            .Where(row => ReferenceEquals(
                row.GetType().GetProperty(scope.RelationshipPropertyName, BindingFlags.Instance | BindingFlags.Public)?.GetValue(row),
                owner))
            .Select(ReadOrdinal)
            .DefaultIfEmpty(0)
            .Max() + 1;

        ordinalProperty.SetValue(rowToAdd, nextOrdinal.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    private static int ReadOrdinal(object row)
    {
        var property = row.GetType().GetProperty("Ordinal", BindingFlags.Instance | BindingFlags.Public);
        var value = property?.GetValue(row) as string;
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : 0;
    }

    private static void AddDataTypeDetails(
        MetaBusinessDataVaultModel model,
        object parentRow,
        BusinessDataVaultAuthoringRequest request)
    {
        if (request.DataTypeDetails.Count == 0)
        {
            return;
        }

        var detailEntityName = $"{request.EntityName}DataTypeDetail";
        var detailType = ResolveEntityType(detailEntityName);
        var detailRows = GetEntityRows(model, detailType, detailEntityName);
        var parentRelationshipProperty = request.EntityName;

        foreach (var detail in request.DataTypeDetails.OrderBy(static item => item.Key, StringComparer.Ordinal))
        {
            var detailId = $"{request.RecordId}:datatype-detail:{detail.Key.ToLowerInvariant()}";
            if (detailRows.Cast<object>().Any(row => string.Equals(ReadId(row), detailId, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"{detailEntityName} '{detailId}' already exists.");
            }

            var detailRow = Activator.CreateInstance(detailType)
                ?? throw new InvalidOperationException($"Could not create {detailEntityName} row.");
            SetText(detailRow, "Id", detailId, detailEntityName);
            SetText(detailRow, "Name", detail.Key, detailEntityName);
            SetText(detailRow, "Value", detail.Value, detailEntityName);

            var parentProperty = detailType.GetProperty(parentRelationshipProperty, BindingFlags.Instance | BindingFlags.Public)
                ?? throw new InvalidOperationException($"Entity '{detailEntityName}' does not define relationship '{parentRelationshipProperty}'.");
            if (!CanAssignRelationship(parentProperty.PropertyType, parentRow.GetType()))
            {
                throw new InvalidOperationException(
                    $"Entity '{detailEntityName}' relationship '{parentRelationshipProperty}' cannot reference {request.EntityName}.");
            }

            parentProperty.SetValue(detailRow, parentRow);
            detailRows.Add(detailRow);
        }
    }

    private static void ValidateDomainRules(
        MetaBusinessDataVaultModel model,
        object rowToAdd,
        BusinessDataVaultAuthoringRequest request)
    {
        if (string.Equals(request.EntityName, "BusinessSameAsLink", StringComparison.Ordinal))
        {
            var primaryHub = rowToAdd.GetType().GetProperty("PrimaryHub", BindingFlags.Instance | BindingFlags.Public)?.GetValue(rowToAdd);
            var equivalentHub = rowToAdd.GetType().GetProperty("EquivalentHub", BindingFlags.Instance | BindingFlags.Public)?.GetValue(rowToAdd);

            if (primaryHub is not null && ReferenceEquals(primaryHub, equivalentHub))
            {
                throw new InvalidOperationException("BusinessSameAsLink requires distinct PrimaryHubId and EquivalentHubId.");
            }
        }

        if (string.Equals(request.EntityName, "BusinessLinkRole", StringComparison.Ordinal))
        {
            BusinessDataVaultRules.ValidateLinkRoleNames(model);
        }
        else if (string.Equals(request.EntityName, "BusinessBridgeTraversal", StringComparison.Ordinal))
        {
            var bridge = ((BusinessBridgeTraversal)rowToAdd).BusinessBridge;
            BusinessDataVaultRules.GetBridgeTraversalChain(
                bridge,
                model.BusinessBridgeTraversalList.Where(row => ReferenceEquals(row.BusinessBridge, bridge)));
        }
    }

    private sealed record OrdinalScope(
        string RelationshipPropertyName,
        IReadOnlyList<string> EntityNames);
}
