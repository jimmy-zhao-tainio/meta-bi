using System.Text.RegularExpressions;
using Meta.Core.Domain;
using Meta.Core.Operations;

namespace MetaDataVault.Core;

public sealed record BusinessDataVaultMaterializationResult(
    Workspace Workspace,
    int MaterializedTableCount,
    int BusinessHubCount,
    int BusinessLinkCount,
    int BusinessHubSatelliteCount,
    int BusinessLinkSatelliteCount,
    int BusinessPointInTimeCount,
    int BusinessBridgeCount);

public interface IBusinessDataVaultMaterializer
{
    Task<BusinessDataVaultMaterializationResult> MaterializeAsync(
        Workspace businessWorkspace,
        Workspace businessDataVaultWorkspace,
        Workspace implementationWorkspace,
        IReadOnlyCollection<Workspace> weaveWorkspaces,
        IReadOnlyCollection<Workspace> fabricWorkspaces,
        string newWorkspaceRootPath,
        CancellationToken cancellationToken = default);
}

public sealed class BusinessDataVaultMaterializer : IBusinessDataVaultMaterializer
{
    private static readonly Regex TokenPattern = new(@"\{(?<name>[A-Za-z][A-Za-z0-9]*)\}", RegexOptions.Compiled);

    private readonly IBusinessDataVaultMaterializationContractService _contractService;

    public BusinessDataVaultMaterializer()
        : this(new BusinessDataVaultMaterializationContractService())
    {
    }

    public BusinessDataVaultMaterializer(IBusinessDataVaultMaterializationContractService contractService)
    {
        _contractService = contractService ?? throw new ArgumentNullException(nameof(contractService));
    }

    public async Task<BusinessDataVaultMaterializationResult> MaterializeAsync(
        Workspace businessWorkspace,
        Workspace businessDataVaultWorkspace,
        Workspace implementationWorkspace,
        IReadOnlyCollection<Workspace> weaveWorkspaces,
        IReadOnlyCollection<Workspace> fabricWorkspaces,
        string newWorkspaceRootPath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(businessWorkspace);
        ArgumentNullException.ThrowIfNull(businessDataVaultWorkspace);
        ArgumentNullException.ThrowIfNull(implementationWorkspace);
        ArgumentNullException.ThrowIfNull(weaveWorkspaces);
        ArgumentNullException.ThrowIfNull(fabricWorkspaces);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspaceRootPath);

        var contract = await _contractService.CheckAsync(
                businessWorkspace,
                businessDataVaultWorkspace,
                implementationWorkspace,
                weaveWorkspaces,
                fabricWorkspaces,
                cancellationToken)
            .ConfigureAwait(false);
        if (contract.HasErrors)
        {
            var message = string.Join(" ", contract.Errors);
            throw new InvalidOperationException($"Business Data Vault materialization contract failed: {message}");
        }

        var outputWorkspace = MetaDataVaultWorkspaceFactory.CreateEmptyWorkspace(
            newWorkspaceRootPath,
            WorkspaceSnapshotCloner.CloneModel(businessDataVaultWorkspace.Model));
        outputWorkspace.WorkspaceConfig = WorkspaceSnapshotCloner.CloneWorkspaceConfig(businessDataVaultWorkspace.WorkspaceConfig);
        outputWorkspace.Instance = WorkspaceSnapshotCloner.CloneInstance(businessDataVaultWorkspace.Instance);
        outputWorkspace.Instance.ModelName = outputWorkspace.Model.Name;
        outputWorkspace.IsDirty = true;

        var hubImplementation = GetSingleImplementationRecord(implementationWorkspace, "BusinessHubImplementation");
        var linkImplementation = GetSingleImplementationRecord(implementationWorkspace, "BusinessLinkImplementation");
        var hubSatelliteImplementation = GetSingleImplementationRecord(implementationWorkspace, "BusinessHubSatelliteImplementation");
        var linkSatelliteImplementation = GetSingleImplementationRecord(implementationWorkspace, "BusinessLinkSatelliteImplementation");
        var pointInTimeImplementation = GetSingleImplementationRecord(implementationWorkspace, "BusinessPointInTimeImplementation");
        var bridgeImplementation = GetSingleImplementationRecord(implementationWorkspace, "BusinessBridgeImplementation");

        var inputHubNames = BuildNameMap(businessDataVaultWorkspace, "BusinessHub");
        var inputLinkNames = BuildNameMap(businessDataVaultWorkspace, "BusinessLink");

        var materializedNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var businessHubs = outputWorkspace.Instance.GetOrCreateEntityRecords("BusinessHub");
        var businessLinks = outputWorkspace.Instance.GetOrCreateEntityRecords("BusinessLink");
        var businessHubSatellites = outputWorkspace.Instance.GetOrCreateEntityRecords("BusinessHubSatellite");
        var businessLinkSatellites = outputWorkspace.Instance.GetOrCreateEntityRecords("BusinessLinkSatellite");
        var businessPointInTimes = outputWorkspace.Instance.GetOrCreateEntityRecords("BusinessPointInTime");
        var businessBridges = outputWorkspace.Instance.GetOrCreateEntityRecords("BusinessBridge");

        foreach (var record in businessHubs)
        {
            MaterializeName(
                "BusinessHub",
                record,
                hubImplementation,
                materializedNames,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["Name"] = RequireName(inputHubNames, "BusinessHub", record.Id),
                });
        }

        foreach (var record in businessLinks)
        {
            MaterializeName(
                "BusinessLink",
                record,
                linkImplementation,
                materializedNames,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["Name"] = RequireName(inputLinkNames, "BusinessLink", record.Id),
                });
        }

        foreach (var record in businessHubSatellites)
        {
            var parentHubId = RequireRelationshipId(record, "BusinessHubId");
            MaterializeName(
                "BusinessHubSatellite",
                record,
                hubSatelliteImplementation,
                materializedNames,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["Name"] = RequireValue(record, "Name"),
                    ["ParentName"] = RequireName(inputHubNames, "BusinessHub", parentHubId),
                });
        }

        foreach (var record in businessLinkSatellites)
        {
            var parentLinkId = RequireRelationshipId(record, "BusinessLinkId");
            MaterializeName(
                "BusinessLinkSatellite",
                record,
                linkSatelliteImplementation,
                materializedNames,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["Name"] = RequireValue(record, "Name"),
                    ["ParentName"] = RequireName(inputLinkNames, "BusinessLink", parentLinkId),
                });
        }

        foreach (var record in businessPointInTimes)
        {
            MaterializeName(
                "BusinessPointInTime",
                record,
                pointInTimeImplementation,
                materializedNames,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["Name"] = RequireValue(record, "Name"),
                });
        }

        foreach (var record in businessBridges)
        {
            MaterializeName(
                "BusinessBridge",
                record,
                bridgeImplementation,
                materializedNames,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["Name"] = RequireValue(record, "Name"),
                });
        }

        return new BusinessDataVaultMaterializationResult(
            outputWorkspace,
            materializedNames.Count,
            businessHubs.Count,
            businessLinks.Count,
            businessHubSatellites.Count,
            businessLinkSatellites.Count,
            businessPointInTimes.Count,
            businessBridges.Count);
    }

    private static void MaterializeName(
        string entityName,
        GenericRecord record,
        GenericRecord implementationRecord,
        IDictionary<string, string> materializedNames,
        IReadOnlyDictionary<string, string> tokens)
    {
        var pattern = RequireValue(implementationRecord, "TableNamePattern");
        var materializedName = RenderPattern(pattern, tokens, entityName, record.Id);
        if (materializedNames.TryGetValue(materializedName, out var previous))
        {
            throw new InvalidOperationException(
                $"Materialized table name '{materializedName}' for '{entityName}:{record.Id}' collides with '{previous}'.");
        }

        materializedNames[materializedName] = $"{entityName}:{record.Id}";
        record.Values["Name"] = materializedName;
    }

    private static string RenderPattern(
        string pattern,
        IReadOnlyDictionary<string, string> tokens,
        string entityName,
        string recordId)
    {
        return TokenPattern.Replace(
            pattern,
            match =>
            {
                var tokenName = match.Groups["name"].Value;
                if (!tokens.TryGetValue(tokenName, out var tokenValue) || string.IsNullOrWhiteSpace(tokenValue))
                {
                    throw new InvalidOperationException(
                        $"Pattern '{pattern}' for '{entityName}:{recordId}' references missing token '{tokenName}'.");
                }

                return tokenValue;
            });
    }

    private static GenericRecord GetSingleImplementationRecord(Workspace implementationWorkspace, string entityName)
    {
        var rows = implementationWorkspace.Instance.GetOrCreateEntityRecords(entityName)
            .OrderBy(record => record.Id, StringComparer.Ordinal)
            .ToList();
        if (rows.Count == 0)
        {
            throw new InvalidOperationException($"MetaDataVaultImplementation is missing required '{entityName}' rows.");
        }

        if (rows.Count > 1)
        {
            throw new InvalidOperationException(
                $"MetaDataVaultImplementation currently requires exactly one '{entityName}' row, found {rows.Count}.");
        }

        return rows[0];
    }

    private static Dictionary<string, string> BuildNameMap(Workspace workspace, string entityName)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName)
            .ToDictionary(
                record => record.Id,
                record => RequireValue(record, "Name"),
                StringComparer.Ordinal);
    }

    private static string RequireName(IReadOnlyDictionary<string, string> names, string entityName, string recordId)
    {
        if (!names.TryGetValue(recordId, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Entity '{entityName}' is missing required Name for row '{recordId}'.");
        }

        return value;
    }

    private static string RequireValue(GenericRecord record, string propertyName)
    {
        if (!record.Values.TryGetValue(propertyName, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Record '{record.Id}' is missing required property '{propertyName}'.");
        }

        return value;
    }

    private static string RequireRelationshipId(GenericRecord record, string relationshipName)
    {
        if (!record.RelationshipIds.TryGetValue(relationshipName, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Record '{record.Id}' is missing required relationship '{relationshipName}'.");
        }

        return value;
    }
}
