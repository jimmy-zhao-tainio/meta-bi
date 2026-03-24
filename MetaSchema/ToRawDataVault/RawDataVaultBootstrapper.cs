using Meta.Core.Domain;
using MS = global::MetaSchema;

namespace MetaSchema.ToRawDataVault;

public sealed partial class RawDataVaultBootstrapper
{
    private const string StandardSatelliteKind = "standard";
    private const string StandardLinkKind = "standard";

    public RawDataVaultBootstrapResult BootstrapWithReport(
        MS.MetaSchemaModel metaSchemaModel,
        string newWorkspacePath,
        RawDataVaultImplementationModel implementation,
        IEnumerable<string>? ignoredFieldNames = null,
        IEnumerable<string>? ignoredFieldSuffixes = null,
        bool includeViews = false)
    {
        ArgumentNullException.ThrowIfNull(metaSchemaModel);
        ArgumentNullException.ThrowIfNull(implementation);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);

        var options = CreateOptions(ignoredFieldNames, ignoredFieldSuffixes, includeViews);
        var draft = ConvertSchemaBootstrap(metaSchemaModel, implementation, options);
        var workspace = CreateWorkspace(draft, newWorkspacePath);
        return new RawDataVaultBootstrapResult(workspace, draft.MaterializationReport);
    }

    public Workspace Bootstrap(
        MS.MetaSchemaModel metaSchemaModel,
        string newWorkspacePath,
        RawDataVaultImplementationModel implementation,
        IEnumerable<string>? ignoredFieldNames = null,
        IEnumerable<string>? ignoredFieldSuffixes = null,
        bool includeViews = false)
    {
        return BootstrapWithReport(
            metaSchemaModel,
            newWorkspacePath,
            implementation,
            ignoredFieldNames,
            ignoredFieldSuffixes,
            includeViews).Workspace;
    }

    private static MetaSchemaBootstrapOptions CreateOptions(
        IEnumerable<string>? ignoredFieldNames,
        IEnumerable<string>? ignoredFieldSuffixes,
        bool includeViews)
    {
        var ignoredFieldNameSet = ignoredFieldNames?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToHashSet(StringComparer.OrdinalIgnoreCase)
            ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ignoredFieldSuffixSet = ignoredFieldSuffixes?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToHashSet(StringComparer.OrdinalIgnoreCase)
            ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        return new MetaSchemaBootstrapOptions(ignoredFieldNameSet, ignoredFieldSuffixSet, includeViews);
    }

    public sealed record RawDataVaultBootstrapResult(
        Workspace Workspace,
        string MaterializationReport);
}
