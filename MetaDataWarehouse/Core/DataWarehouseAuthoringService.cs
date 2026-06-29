using System.Collections;
using System.Reflection;

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

public sealed record DataWarehouseWorkspaceCreationResult(
    string WorkspacePath,
    string ModelName,
    int RowCount);

public interface IDataWarehouseAuthoringService
{
    DataWarehouseWorkspaceCreationResult CreateWorkspace(string workspacePath);

    MetaDataWarehouseModel AddRecord(DataWarehouseAuthoringRequest request);
}

public sealed class DataWarehouseAuthoringService : IDataWarehouseAuthoringService
{
    private const string ModelName = "MetaDataWarehouse";

    private static readonly Type ModelType = typeof(MetaDataWarehouseModel);

    public DataWarehouseWorkspaceCreationResult CreateWorkspace(string workspacePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);

        var fullPath = MetaDataWarehouseTooling.CreateWorkspace(Path.GetFullPath(workspacePath));
        return new DataWarehouseWorkspaceCreationResult(fullPath, ModelName, RowCount: 0);
    }

    public MetaDataWarehouseModel AddRecord(DataWarehouseAuthoringRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);

        var workspacePath = Path.GetFullPath(request.WorkspacePath);
        var model = MetaDataWarehouseTooling.Load(workspacePath);
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
        var type = ModelType.Assembly.GetType($"MetaDataWarehouse.{entityName}", throwOnError: false);
        if (type is null)
        {
            throw new InvalidOperationException($"Entity '{entityName}' was not found in model '{ModelName}'.");
        }

        return type;
    }

    private static IList GetEntityRows(MetaDataWarehouseModel model, Type entityType, string entityName)
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

    private static object ResolveRow(MetaDataWarehouseModel model, string entityName, string id)
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
        MetaDataWarehouseModel model,
        object row,
        string entityName,
        DataWarehouseRelationshipAssignment relationship)
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
        MetaDataWarehouseModel model,
        object rowToAdd,
        DataWarehouseAuthoringRequest request)
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

    private static void ValidateDomainRules(MetaDataWarehouseModel model, object row)
    {
        switch (row)
        {
            case DimensionBusinessKeyPart keyPart:
                ValidateRelatedDimensionAttribute(
                    nameof(DimensionBusinessKeyPart),
                    keyPart.Id,
                    keyPart.DimensionBusinessKey.Id,
                    keyPart.DimensionBusinessKey.Dimension,
                    keyPart.DimensionAttribute);
                break;
            case Type1DimensionAttribute type1Attribute:
                ValidateRelatedDimensionAttribute(
                    nameof(Type1DimensionAttribute),
                    type1Attribute.Id,
                    type1Attribute.SlowlyChangingDimension.Id,
                    type1Attribute.SlowlyChangingDimension.Dimension,
                    type1Attribute.DimensionAttribute);
                break;
            case Type2DimensionAttribute type2Attribute:
                ValidateRelatedDimensionAttribute(
                    nameof(Type2DimensionAttribute),
                    type2Attribute.Id,
                    type2Attribute.SlowlyChangingDimension.Id,
                    type2Attribute.SlowlyChangingDimension.Dimension,
                    type2Attribute.DimensionAttribute);
                break;
            case DimensionHierarchyLevel hierarchyLevel:
                ValidateRelatedDimensionAttribute(
                    nameof(DimensionHierarchyLevel),
                    hierarchyLevel.Id,
                    hierarchyLevel.DimensionHierarchy.Id,
                    hierarchyLevel.DimensionHierarchy.Dimension,
                    hierarchyLevel.DimensionAttribute);
                break;
            case JunkDimensionComponent junkComponent:
                ValidateRelatedDimensionAttribute(
                    nameof(JunkDimensionComponent),
                    junkComponent.Id,
                    junkComponent.JunkDimension.Id,
                    junkComponent.JunkDimension.Dimension,
                    junkComponent.DimensionAttribute);
                break;
            case FactDimension factDimension:
                ValidateUniqueFactDimensionRole(model, factDimension);
                break;
            case FactBridge factBridge:
                ValidateUniqueFactBridgeRole(model, factBridge);
                break;
            case BridgeParticipant bridgeParticipant:
                ValidateUniqueBridgeParticipantRole(model, bridgeParticipant);
                ValidateUniqueBridgeParticipantOrdinal(model, bridgeParticipant);
                break;
            case AggregateFact aggregateFact:
                if (ReferenceEquals(aggregateFact.AggregatedFact, aggregateFact.SourceFact))
                {
                    throw new InvalidOperationException("AggregateFact requires distinct AggregatedFact and SourceFact.");
                }

                break;
        }
    }

    private static void ValidateRelatedDimensionAttribute(
        string entityName,
        string id,
        string ownerId,
        Dimension ownerDimension,
        DimensionAttribute attribute)
    {
        if (!ReferenceEquals(ownerDimension, attribute.Dimension))
        {
            throw new InvalidOperationException(
                $"{entityName} '{id}' references attribute '{attribute.Id}' from dimension '{attribute.Dimension.Id}', but owner '{ownerId}' belongs to dimension '{ownerDimension.Id}'.");
        }
    }

    private static void ValidateUniqueFactDimensionRole(MetaDataWarehouseModel model, FactDimension factDimension)
    {
        if (string.IsNullOrWhiteSpace(factDimension.RoleName))
        {
            return;
        }

        var duplicate = model.FactDimensionList.Any(other =>
            !ReferenceEquals(other, factDimension) &&
            ReferenceEquals(other.Fact, factDimension.Fact) &&
            string.Equals(other.RoleName, factDimension.RoleName, StringComparison.OrdinalIgnoreCase));
        if (duplicate)
        {
            throw new InvalidOperationException($"FactDimension owner '{factDimension.Fact.Id}' already contains role '{factDimension.RoleName}'.");
        }
    }

    private static void ValidateUniqueFactBridgeRole(MetaDataWarehouseModel model, FactBridge factBridge)
    {
        if (string.IsNullOrWhiteSpace(factBridge.RoleName))
        {
            return;
        }

        var duplicate = model.FactBridgeList.Any(other =>
            !ReferenceEquals(other, factBridge) &&
            ReferenceEquals(other.Fact, factBridge.Fact) &&
            string.Equals(other.RoleName, factBridge.RoleName, StringComparison.OrdinalIgnoreCase));
        if (duplicate)
        {
            throw new InvalidOperationException($"FactBridge owner '{factBridge.Fact.Id}' already contains role '{factBridge.RoleName}'.");
        }
    }

    private static void ValidateUniqueBridgeParticipantRole(MetaDataWarehouseModel model, BridgeParticipant participant)
    {
        if (string.IsNullOrWhiteSpace(participant.RoleName))
        {
            return;
        }

        var duplicate = model.BridgeParticipantList.Any(other =>
            !ReferenceEquals(other, participant) &&
            ReferenceEquals(other.BridgeTable, participant.BridgeTable) &&
            string.Equals(other.RoleName, participant.RoleName, StringComparison.OrdinalIgnoreCase));
        if (duplicate)
        {
            throw new InvalidOperationException($"BridgeParticipant owner '{participant.BridgeTable.Id}' already contains role '{participant.RoleName}'.");
        }
    }

    private static void ValidateUniqueBridgeParticipantOrdinal(MetaDataWarehouseModel model, BridgeParticipant participant)
    {
        if (string.IsNullOrWhiteSpace(participant.Ordinal))
        {
            return;
        }

        var duplicate = model.BridgeParticipantList.Any(other =>
            !ReferenceEquals(other, participant) &&
            ReferenceEquals(other.BridgeTable, participant.BridgeTable) &&
            string.Equals(other.Ordinal, participant.Ordinal, StringComparison.Ordinal));
        if (duplicate)
        {
            throw new InvalidOperationException($"BridgeParticipant owner '{participant.BridgeTable.Id}' already contains ordinal '{participant.Ordinal}'.");
        }
    }
}
