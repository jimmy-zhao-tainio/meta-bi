using Meta.Integration;
using Meta.Operations;
using Meta.Operations.Domain;

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
    MetaDataWarehouseModel CreateWorkspace();

    MetaDataWarehouseModel AddRecord(DataWarehouseAuthoringRequest request);
}

public sealed class DataWarehouseAuthoringService : IDataWarehouseAuthoringService
{
    private const string ModelName = "MetaDataWarehouse";

    public MetaDataWarehouseModel CreateWorkspace() => MetaDataWarehouseModel.CreateEmpty();

    public MetaDataWarehouseModel AddRecord(DataWarehouseAuthoringRequest request) =>
        AddRecordAsync(request).GetAwaiter().GetResult();

    private static async Task<MetaDataWarehouseModel> AddRecordAsync(DataWarehouseAuthoringRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);
        await using var workspace = await WorkspaceSurface.OpenAsync(Path.GetFullPath(request.WorkspacePath)).ConfigureAwait(false);
        var state = await WorkspaceComposition.MaterializeAsync(workspace).ConfigureAwait(false);
        var binding = new MetaDataWarehouseWorkspaceBinding();
        _ = binding.Read(state);
        var values = new Dictionary<string, string>(request.Values, StringComparer.OrdinalIgnoreCase);
        var relationships = request.Relationships.Select(item => (item.ColumnName, item.TargetEntityName, item.TargetRecordId)).ToList();
        var operations = new List<Operation>();
        operations.Add(BuildInsert(state, request.EntityName, request.RecordId, values, relationships));
        var candidate = InMemoryOperations.ApplyBatch(state, operations);
        var model = binding.Read(candidate);
        object? added = state.Model.FindEntity(request.EntityName)!.Name switch
        {
            nameof(DimensionBusinessKeyPart) => model.DimensionBusinessKeyPartList.Single(row => row.Id == request.RecordId),
            nameof(Type1DimensionAttribute) => model.Type1DimensionAttributeList.Single(row => row.Id == request.RecordId),
            nameof(Type2DimensionAttribute) => model.Type2DimensionAttributeList.Single(row => row.Id == request.RecordId),
            nameof(DimensionHierarchyLevel) => model.DimensionHierarchyLevelList.Single(row => row.Id == request.RecordId),
            nameof(JunkDimensionComponent) => model.JunkDimensionComponentList.Single(row => row.Id == request.RecordId),
            nameof(FactDimension) => model.FactDimensionList.Single(row => row.Id == request.RecordId),
            nameof(FactBridge) => model.FactBridgeList.Single(row => row.Id == request.RecordId),
            nameof(BridgeParticipant) => model.BridgeParticipantList.Single(row => row.Id == request.RecordId),
            nameof(AggregateFact) => model.AggregateFactList.Single(row => row.Id == request.RecordId),
            _ => null,
        };
        if (added is not null) ValidateDomainRules(model, added);
        await workspace.ExecuteAsync(operations).ConfigureAwait(false);
        return model;
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
