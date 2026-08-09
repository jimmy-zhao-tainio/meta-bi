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

public sealed class BusinessDataVaultSatelliteRequest
{
    public required string WorkspacePath { get; init; }
    public required string SatelliteEntityName { get; init; }
    public required string ParentEntityName { get; init; }
    public required string ParentRecordId { get; init; }
    public required string RecordId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}

public sealed record BusinessDataVaultRelationshipAssignment(string ColumnName, string TargetEntityName, string TargetRecordId);

public interface IBusinessDataVaultAuthoringService
{
    MetaBusinessDataVaultModel CreateWorkspace();

    MetaBusinessDataVaultModel AddRecord(BusinessDataVaultAuthoringRequest request);

    MetaBusinessDataVaultModel AddSatellite(BusinessDataVaultSatelliteRequest request);
}

public sealed class BusinessDataVaultAuthoringService : IBusinessDataVaultAuthoringService
{
    private const string ModelName = "MetaBusinessDataVault";

    private static readonly Type ModelType = typeof(MetaBusinessDataVaultModel);

    public MetaBusinessDataVaultModel CreateWorkspace() => MetaBusinessDataVaultModel.CreateEmpty();

    public MetaBusinessDataVaultModel AddRecord(BusinessDataVaultAuthoringRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var model = Meta.Core.Serialization.TypedWorkspaceModelMapper.Load<MetaBusinessDataVaultModel>(workspacePath);
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
        AddDataTypeDetails(model, rowToAdd, request);
        ValidateDomainRules(model, rowToAdd, request);
        BusinessDataVaultRules.ValidateSatelliteSpecializations(model);

        Meta.Core.Serialization.TypedWorkspaceModelMapper.Save(model, workspacePath);
        return model;
    }

    public MetaBusinessDataVaultModel AddSatellite(BusinessDataVaultSatelliteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.SatelliteEntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ParentEntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ParentRecordId);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var model = Meta.Core.Serialization.TypedWorkspaceModelMapper.Load<MetaBusinessDataVaultModel>(workspacePath);
        var baseEntityType = ResolveEntityType("BusinessSatellite");
        var baseRows = GetEntityRows(model, baseEntityType, "BusinessSatellite");
        var satelliteEntityType = ResolveEntityType(request.SatelliteEntityName);
        var satelliteRows = GetEntityRows(model, satelliteEntityType, request.SatelliteEntityName);

        if (baseRows.Cast<object>().Any(row => string.Equals(ReadId(row), request.RecordId, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException($"BusinessSatellite '{request.RecordId}' already exists.");
        }

        if (satelliteRows.Cast<object>().Any(row => string.Equals(ReadId(row), request.RecordId, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException($"{request.SatelliteEntityName} '{request.RecordId}' already exists.");
        }

        var baseRow = Activator.CreateInstance(baseEntityType)
            ?? throw new InvalidOperationException("Could not create BusinessSatellite row.");
        SetText(baseRow, "Id", request.RecordId, "BusinessSatellite");
        SetText(baseRow, "Name", request.Name, "BusinessSatellite");
        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            SetText(baseRow, "Description", request.Description, "BusinessSatellite");
        }

        baseRows.Add(baseRow);

        var satelliteRow = Activator.CreateInstance(satelliteEntityType)
            ?? throw new InvalidOperationException($"Could not create {request.SatelliteEntityName} row.");
        SetText(satelliteRow, "Id", request.RecordId, request.SatelliteEntityName);
        AssignRelationship(
            model,
            satelliteRow,
            request.SatelliteEntityName,
            new BusinessDataVaultRelationshipAssignment("BusinessSatelliteId", "BusinessSatellite", request.RecordId));
        AssignRelationship(
            model,
            satelliteRow,
            request.SatelliteEntityName,
            new BusinessDataVaultRelationshipAssignment(
                $"{request.ParentEntityName}Id",
                request.ParentEntityName,
                request.ParentRecordId));

        satelliteRows.Add(satelliteRow);
        BusinessDataVaultRules.ValidateSatelliteSpecializations(model);

        Meta.Core.Serialization.TypedWorkspaceModelMapper.Save(model, workspacePath);
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

        if (string.Equals(request.EntityName, "BusinessHubKeyPart", StringComparison.Ordinal))
        {
            var hub = ((BusinessHubKeyPart)rowToAdd).BusinessHub;
            BusinessDataVaultRules.GetHubKeyPartChain(
                hub,
                model.BusinessHubKeyPartList.Where(row => ReferenceEquals(row.BusinessHub, hub)));
        }
        else if (string.Equals(request.EntityName, "BusinessReferenceKeyPart", StringComparison.Ordinal))
        {
            var reference = ((BusinessReferenceKeyPart)rowToAdd).BusinessReference;
            BusinessDataVaultRules.GetReferenceKeyPartChain(
                reference,
                model.BusinessReferenceKeyPartList.Where(row => ReferenceEquals(row.BusinessReference, reference)));
        }
        else if (string.Equals(request.EntityName, "BusinessLinkRole", StringComparison.Ordinal))
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
}
