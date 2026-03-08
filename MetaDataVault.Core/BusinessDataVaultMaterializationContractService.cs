using Meta.Core.Domain;
using MetaFabric.Core;
using MetaWeave.Core;

namespace MetaDataVault.Core;

public sealed record BusinessDataVaultMaterializationContractResult(
    int WeaveCount,
    int FabricCount,
    int FlatAnchorsSatisfied,
    int FlatAnchorsRequired,
    int ScopedAnchorsSatisfied,
    int ScopedAnchorsRequired,
    IReadOnlyList<string> Errors)
{
    public bool HasErrors => Errors.Count > 0;
}

public interface IBusinessDataVaultMaterializationContractService
{
    Task<BusinessDataVaultMaterializationContractResult> CheckAsync(
        Workspace businessWorkspace,
        Workspace businessDataVaultWorkspace,
        Workspace implementationWorkspace,
        IReadOnlyCollection<Workspace> weaveWorkspaces,
        IReadOnlyCollection<Workspace> fabricWorkspaces,
        CancellationToken cancellationToken = default);
}

public sealed class BusinessDataVaultMaterializationContractService : IBusinessDataVaultMaterializationContractService
{
    private static readonly BindingSignature[] RequiredFlatAnchors =
    {
        new("BusinessHub", "Name", "BusinessObject", "Name"),
        new("BusinessLink", "Name", "BusinessRelationship", "Name"),
    };

    private static readonly BindingSignature[] RequiredScopedAnchors =
    {
        new("BusinessLinkEnd", "RoleName", "BusinessRelationshipParticipant", "RoleName"),
        new("BusinessHubKeyPart", "Name", "BusinessKeyPart", "Name"),
    };

    private static readonly string[] RequiredImplementationEntities =
    {
        "BusinessHubImplementation",
        "BusinessLinkImplementation",
        "BusinessHubSatelliteImplementation",
        "BusinessLinkSatelliteImplementation",
        "BusinessPointInTimeImplementation",
        "BusinessBridgeImplementation",
    };

    private readonly IMetaWeaveService _metaWeaveService;
    private readonly IMetaFabricService _metaFabricService;

    public BusinessDataVaultMaterializationContractService()
        : this(new MetaWeaveService(), new MetaFabricService())
    {
    }

    public BusinessDataVaultMaterializationContractService(
        IMetaWeaveService metaWeaveService,
        IMetaFabricService metaFabricService)
    {
        _metaWeaveService = metaWeaveService ?? throw new ArgumentNullException(nameof(metaWeaveService));
        _metaFabricService = metaFabricService ?? throw new ArgumentNullException(nameof(metaFabricService));
    }

    public async Task<BusinessDataVaultMaterializationContractResult> CheckAsync(
        Workspace businessWorkspace,
        Workspace businessDataVaultWorkspace,
        Workspace implementationWorkspace,
        IReadOnlyCollection<Workspace> weaveWorkspaces,
        IReadOnlyCollection<Workspace> fabricWorkspaces,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(businessWorkspace);
        ArgumentNullException.ThrowIfNull(businessDataVaultWorkspace);
        ArgumentNullException.ThrowIfNull(implementationWorkspace);
        ArgumentNullException.ThrowIfNull(weaveWorkspaces);
        ArgumentNullException.ThrowIfNull(fabricWorkspaces);

        EnsureModel(businessWorkspace, "MetaBusiness", nameof(businessWorkspace));
        EnsureModel(businessDataVaultWorkspace, "MetaBusinessDataVault", nameof(businessDataVaultWorkspace));
        EnsureModel(implementationWorkspace, "MetaDataVaultImplementation", nameof(implementationWorkspace));

        var errors = new List<string>();
        var weaveList = weaveWorkspaces.OrderBy(item => item.WorkspaceRootPath, StringComparer.Ordinal).ToList();
        var fabricList = fabricWorkspaces.OrderBy(item => item.WorkspaceRootPath, StringComparer.Ordinal).ToList();

        if (weaveList.Count == 0)
        {
            errors.Add("At least one MetaWeave workspace is required.");
        }

        if (fabricList.Count == 0)
        {
            errors.Add("At least one MetaFabric workspace is required.");
        }

        var scopedFabricBindings = CollectScopedFabricBindings(fabricList, errors);

        foreach (var weaveWorkspace in weaveList)
        {
            EnsureModel(weaveWorkspace, MetaWeaveModels.MetaWeaveModelName, nameof(weaveWorkspaces));
            ValidateWeaveWorkspaceTargets(weaveWorkspace, businessWorkspace, businessDataVaultWorkspace, errors);
            var check = await _metaWeaveService.CheckAsync(weaveWorkspace, cancellationToken).ConfigureAwait(false);
            var normalizedWeavePath = NormalizePath(weaveWorkspace.WorkspaceRootPath);
            var allowedScopedBindings = scopedFabricBindings.BindingNamesByWeavePath.TryGetValue(normalizedWeavePath, out var names)
                ? names
                : null;
            var unsupportedErrors = check.Bindings.Where(binding => binding.Errors.Count > 0 && (allowedScopedBindings == null || !allowedScopedBindings.Contains(binding.BindingName))).ToList();
            if (unsupportedErrors.Count > 0)
            {
                errors.Add($"Weave workspace '{weaveWorkspace.WorkspaceRootPath}' failed weave check outside sanctioned fabric scope.");
            }
        }

        foreach (var fabricWorkspace in fabricList)
        {
            EnsureModel(fabricWorkspace, MetaFabricModels.MetaFabricModelName, nameof(fabricWorkspaces));
            ValidateFabricWorkspaceTargets(fabricWorkspace, weaveList, errors);
            var check = await _metaFabricService.CheckAsync(fabricWorkspace, cancellationToken).ConfigureAwait(false);
            if (check.HasErrors)
            {
                errors.Add($"Fabric workspace '{fabricWorkspace.WorkspaceRootPath}' failed fabric check.");
            }
        }

        foreach (var entityName in RequiredImplementationEntities)
        {
            if (implementationWorkspace.Instance.GetOrCreateEntityRecords(entityName).Count == 0)
            {
                errors.Add($"MetaDataVaultImplementation is missing required '{entityName}' rows.");
            }
        }

        var flatBindings = CollectWeaveBindingSignatures(weaveList);
        var scopedBindings = scopedFabricBindings.Signatures;
        var flatSatisfied = RequiredFlatAnchors.Count(flatBindings.Contains);
        var scopedSatisfied = RequiredScopedAnchors.Count(scopedBindings.Contains);

        foreach (var missing in RequiredFlatAnchors.Where(item => !flatBindings.Contains(item)))
        {
            errors.Add($"Missing flat anchor '{missing}'.");
        }

        foreach (var missing in RequiredScopedAnchors.Where(item => !scopedBindings.Contains(item)))
        {
            errors.Add($"Missing scoped anchor '{missing}'.");
        }

        return new BusinessDataVaultMaterializationContractResult(
            weaveList.Count,
            fabricList.Count,
            flatSatisfied,
            RequiredFlatAnchors.Length,
            scopedSatisfied,
            RequiredScopedAnchors.Length,
            errors);
    }

    private static void ValidateWeaveWorkspaceTargets(
        Workspace weaveWorkspace,
        Workspace businessWorkspace,
        Workspace businessDataVaultWorkspace,
        ICollection<string> errors)
    {
        var modelReferences = weaveWorkspace.Instance.GetOrCreateEntityRecords("ModelReference").ToList();
        ValidateReferencedModelPath(weaveWorkspace, modelReferences, "MetaBusiness", businessWorkspace.WorkspaceRootPath, errors);
        ValidateReferencedModelPath(weaveWorkspace, modelReferences, "MetaBusinessDataVault", businessDataVaultWorkspace.WorkspaceRootPath, errors);
    }

    private static void ValidateReferencedModelPath(
        Workspace weaveWorkspace,
        IReadOnlyCollection<GenericRecord> modelReferences,
        string expectedModelName,
        string expectedWorkspaceRootPath,
        ICollection<string> errors)
    {
        var matchingReferences = modelReferences.Where(record =>
                record.Values.TryGetValue("ModelName", out var modelName) &&
                string.Equals(modelName, expectedModelName, StringComparison.Ordinal))
            .ToList();
        if (matchingReferences.Count == 0)
        {
            errors.Add($"Weave workspace '{weaveWorkspace.WorkspaceRootPath}' does not reference sanctioned model '{expectedModelName}'.");
            return;
        }

        foreach (var record in matchingReferences)
        {
            var resolvedPath = ResolveWorkspacePath(weaveWorkspace.WorkspaceRootPath, RequireValue(record, "WorkspacePath"));
            if (!string.Equals(NormalizePath(resolvedPath), NormalizePath(expectedWorkspaceRootPath), StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"Weave workspace '{weaveWorkspace.WorkspaceRootPath}' references '{expectedModelName}' at '{resolvedPath}', expected '{expectedWorkspaceRootPath}'.");
            }
        }
    }

    private static void ValidateFabricWorkspaceTargets(
        Workspace fabricWorkspace,
        IReadOnlyCollection<Workspace> weaveWorkspaces,
        ICollection<string> errors)
    {
        var expectedPaths = weaveWorkspaces
            .Select(item => NormalizePath(item.WorkspaceRootPath))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var weaveReference in fabricWorkspace.Instance.GetOrCreateEntityRecords("WeaveReference"))
        {
            var resolvedPath = ResolveWorkspacePath(fabricWorkspace.WorkspaceRootPath, RequireValue(weaveReference, "WorkspacePath"));
            if (!expectedPaths.Contains(NormalizePath(resolvedPath)))
            {
                errors.Add($"Fabric workspace '{fabricWorkspace.WorkspaceRootPath}' references weave '{resolvedPath}' outside the provided weave set.");
            }
        }
    }

    private static HashSet<BindingSignature> CollectWeaveBindingSignatures(IReadOnlyCollection<Workspace> weaveWorkspaces)
    {
        var signatures = new HashSet<BindingSignature>();
        foreach (var weaveWorkspace in weaveWorkspaces)
        {
            foreach (var binding in weaveWorkspace.Instance.GetOrCreateEntityRecords("PropertyBinding"))
            {
                signatures.Add(new BindingSignature(
                    RequireValue(binding, "SourceEntity"),
                    RequireValue(binding, "SourceProperty"),
                    RequireValue(binding, "TargetEntity"),
                    RequireValue(binding, "TargetProperty")));
            }
        }

        return signatures;
    }

    private static ScopedFabricBindingIndex CollectScopedFabricBindings(
        IReadOnlyCollection<Workspace> fabricWorkspaces,
        ICollection<string> errors)
    {
        var signatures = new HashSet<BindingSignature>();
        var bindingNamesByWeavePath = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var fabricWorkspace in fabricWorkspaces)
        {
            var weavePathsByReferenceId = fabricWorkspace.Instance.GetOrCreateEntityRecords("WeaveReference")
                .ToDictionary(
                    record => record.Id,
                    record => NormalizePath(ResolveWorkspacePath(fabricWorkspace.WorkspaceRootPath, RequireValue(record, "WorkspacePath"))),
                    StringComparer.Ordinal);
            var scopedBindingIds = fabricWorkspace.Instance.GetOrCreateEntityRecords("BindingScopeRequirement")
                .Select(record => RequireRelationshipId(record, "BindingId"))
                .ToHashSet(StringComparer.Ordinal);

            foreach (var bindingReference in fabricWorkspace.Instance.GetOrCreateEntityRecords("BindingReference"))
            {
                if (!scopedBindingIds.Contains(bindingReference.Id))
                {
                    continue;
                }

                var weaveReferenceId = RequireRelationshipId(bindingReference, "WeaveReferenceId");
                if (!weavePathsByReferenceId.TryGetValue(weaveReferenceId, out var weavePath))
                {
                    errors.Add($"Fabric workspace '{fabricWorkspace.WorkspaceRootPath}' references missing weave id '{weaveReferenceId}'.");
                    continue;
                }

                var bindingName = RequireValue(bindingReference, "BindingName");
                if (!bindingNamesByWeavePath.TryGetValue(weavePath, out var names))
                {
                    names = new HashSet<string>(StringComparer.Ordinal);
                    bindingNamesByWeavePath[weavePath] = names;
                }

                names.Add(bindingName);
                if (!BindingSignature.TryParse(bindingName, out var signature))
                {
                    errors.Add($"Fabric workspace '{fabricWorkspace.WorkspaceRootPath}' contains unparseable binding name '{bindingName}'.");
                    continue;
                }

                signatures.Add(signature);
            }
        }

        return new ScopedFabricBindingIndex(bindingNamesByWeavePath, signatures);
    }

    private static string ResolveWorkspacePath(string workspaceRootPath, string configuredPath)
    {
        if (Path.IsPathRooted(configuredPath))
        {
            return Path.GetFullPath(configuredPath);
        }

        return Path.GetFullPath(Path.Combine(workspaceRootPath, configuredPath));
    }

    private static string NormalizePath(string path)
    {
        return Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
    }

    private static void EnsureModel(Workspace workspace, string expectedModelName, string parameterName)
    {
        if (!string.Equals(workspace.Model.Name, expectedModelName, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"Expected sanctioned model '{expectedModelName}' but found '{workspace.Model.Name}'.",
                parameterName);
        }
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

    private sealed record ScopedFabricBindingIndex(IReadOnlyDictionary<string, HashSet<string>> BindingNamesByWeavePath, IReadOnlySet<BindingSignature> Signatures);

    private sealed record BindingSignature(string SourceEntity, string SourceProperty, string TargetEntity, string TargetProperty)
    {
        public override string ToString()
        {
            return $"{SourceEntity}.{SourceProperty} -> {TargetEntity}.{TargetProperty}";
        }

        public static bool TryParse(string text, out BindingSignature signature)
        {
            signature = default!;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var parts = text.Split(new[] { "->" }, StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
            {
                return false;
            }

            if (!TryParseSide(parts[0], out var sourceEntity, out var sourceProperty) ||
                !TryParseSide(parts[1], out var targetEntity, out var targetProperty))
            {
                return false;
            }

            signature = new BindingSignature(sourceEntity, sourceProperty, targetEntity, targetProperty);
            return true;
        }

        private static bool TryParseSide(string text, out string entityName, out string propertyName)
        {
            entityName = string.Empty;
            propertyName = string.Empty;
            var index = text.LastIndexOf('.');
            if (index <= 0 || index >= text.Length - 1)
            {
                return false;
            }

            entityName = text[..index].Trim();
            propertyName = text[(index + 1)..].Trim();
            return entityName.Length > 0 && propertyName.Length > 0;
        }
    }
}


