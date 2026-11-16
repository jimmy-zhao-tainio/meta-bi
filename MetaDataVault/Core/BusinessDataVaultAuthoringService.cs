using Meta.Integration;
using Meta.Operations;
using Meta.Operations.Domain;
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

    public MetaBusinessDataVaultModel CreateWorkspace() => MetaBusinessDataVaultModel.CreateEmpty();

    public MetaBusinessDataVaultModel AddRecord(BusinessDataVaultAuthoringRequest request) =>
        AddRecordAsync(request).GetAwaiter().GetResult();

    private static async Task<MetaBusinessDataVaultModel> AddRecordAsync(BusinessDataVaultAuthoringRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);
        await using var workspace = await WorkspaceSurface.OpenAsync(Path.GetFullPath(request.WorkspacePath)).ConfigureAwait(false);
        var state = await WorkspaceComposition.MaterializeAsync(workspace).ConfigureAwait(false);
        var binding = new MetaBusinessDataVaultWorkspaceBinding();
        _ = binding.Read(state);
        var values = new Dictionary<string, string>(request.Values, StringComparer.OrdinalIgnoreCase);
        var relationships = request.Relationships.Select(item => (item.ColumnName, item.TargetEntityName, item.TargetRecordId)).ToList();
        var operations = new List<Operation>();
        operations.Add(BuildInsert(state, request.EntityName, request.RecordId, values, relationships));
        foreach (var detail in request.DataTypeDetails.OrderBy(item => item.Key, StringComparer.Ordinal))
            operations.Add(BuildInsert(state, request.EntityName + "DataTypeDetail",
                request.RecordId + ":datatype-detail:" + detail.Key.ToLowerInvariant(),
                new() { ["Name"] = detail.Key, ["Value"] = detail.Value },
                [(request.EntityName + "Id", request.EntityName, request.RecordId)]));
        var candidate = InMemoryOperations.ApplyBatch(state, operations);
        var model = binding.Read(candidate);
        object? added = state.Model.FindEntity(request.EntityName)!.Name switch
        {
            nameof(BusinessSameAsLink) => model.BusinessSameAsLinkList.Single(row => row.Id == request.RecordId),
            nameof(BusinessHubKeyPart) => model.BusinessHubKeyPartList.Single(row => row.Id == request.RecordId),
            nameof(BusinessReferenceKeyPart) => model.BusinessReferenceKeyPartList.Single(row => row.Id == request.RecordId),
            nameof(BusinessLinkRole) => model.BusinessLinkRoleList.Single(row => row.Id == request.RecordId),
            nameof(BusinessBridgeTraversal) => model.BusinessBridgeTraversalList.Single(row => row.Id == request.RecordId),
            _ => null,
        };
        if (added is not null) ValidateDomainRules(model, added, request);
        await workspace.ExecuteAsync(operations).ConfigureAwait(false);
        return model;
    }

    public MetaBusinessDataVaultModel AddSatellite(BusinessDataVaultSatelliteRequest request) =>
        AddSatelliteAsync(request).GetAwaiter().GetResult();

    private static async Task<MetaBusinessDataVaultModel> AddSatelliteAsync(BusinessDataVaultSatelliteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.SatelliteEntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ParentEntityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ParentRecordId);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecordId);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);
        await using var workspace = await WorkspaceSurface.OpenAsync(Path.GetFullPath(request.WorkspacePath)).ConfigureAwait(false);
        var state = await WorkspaceComposition.MaterializeAsync(workspace).ConfigureAwait(false);
        var binding = new MetaBusinessDataVaultWorkspaceBinding();
        _ = binding.Read(state);
        var values = new Dictionary<string, string> { ["Name"] = request.Name };
        if (!string.IsNullOrWhiteSpace(request.Description)) values["Description"] = request.Description;
        Operation[] operations = [
            BuildInsert(state, nameof(BusinessSatellite), request.RecordId, values, []),
            BuildInsert(state, request.SatelliteEntityName, request.RecordId, new(),
                [("BusinessSatelliteId", nameof(BusinessSatellite), request.RecordId),
                 (request.ParentEntityName + "Id", request.ParentEntityName, request.ParentRecordId)]),
        ];
        var model = binding.Read(InMemoryOperations.ApplyBatch(state, operations));
        BusinessDataVaultRules.ValidateSatelliteSpecializations(model);
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
        return new Operation.InsertRecord(entity.Name, id, values, references);
    }

    private static void ValidateDomainRules(
        MetaBusinessDataVaultModel model,
        object rowToAdd,
        BusinessDataVaultAuthoringRequest request)
    {
        if (string.Equals(request.EntityName, "BusinessSameAsLink", StringComparison.Ordinal))
        {
            var primaryHub = ((BusinessSameAsLink)rowToAdd).PrimaryHub;
            var equivalentHub = ((BusinessSameAsLink)rowToAdd).EquivalentHub;

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
