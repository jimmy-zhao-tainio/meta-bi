using MS = global::MetaSchema;
using MRDV = global::MetaRawDataVault;

namespace MetaDataVault.FromMetaSchema;

public sealed partial class RawDataVaultFromMetaSchemaService
{
    private const string StandardSatelliteKind = "standard";
    private const string StandardLinkKind = "standard";

    public RawDataVaultFromMetaSchemaResult MaterializeWithReport(
        MS.MetaSchemaModel metaSchemaModel,
        IEnumerable<string>? ignoredFieldNames = null,
        IEnumerable<string>? ignoredFieldSuffixes = null,
        bool includeViews = false)
    {
        ArgumentNullException.ThrowIfNull(metaSchemaModel);

        var options = CreateOptions(ignoredFieldNames, ignoredFieldSuffixes, includeViews);
        var (draft, report) = ConvertFromMetaSchema(metaSchemaModel, options);
        var model = CreateModel(draft);
        return new RawDataVaultFromMetaSchemaResult(model, report);
    }

    public MRDV.MetaRawDataVaultModel Materialize(
        MS.MetaSchemaModel metaSchemaModel,
        IEnumerable<string>? ignoredFieldNames = null,
        IEnumerable<string>? ignoredFieldSuffixes = null,
        bool includeViews = false)
    {
        return MaterializeWithReport(
            metaSchemaModel,
            ignoredFieldNames,
            ignoredFieldSuffixes,
            includeViews).Model;
    }

    public async Task<RawDataVaultFromMetaSchemaResult> MaterializeWithReportAsync(
        string sourceWorkspacePath,
        string newWorkspacePath,
        IEnumerable<string>? ignoredFieldNames = null,
        IEnumerable<string>? ignoredFieldSuffixes = null,
        bool includeViews = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);
        cancellationToken.ThrowIfCancellationRequested();

        var sourceWorkspacePathFull = Path.GetFullPath(sourceWorkspacePath);
        var newWorkspacePathFull = Path.GetFullPath(newWorkspacePath);
        EnsureTargetDirectoryIsEmpty(newWorkspacePathFull);

        var sourceModel = await MS.MetaSchemaTooling.LoadAsync(
            sourceWorkspacePathFull,
            searchUpward: false,
            cancellationToken).ConfigureAwait(false);

        var result = MaterializeWithReport(
            sourceModel,
            ignoredFieldNames,
            ignoredFieldSuffixes,
            includeViews);

        await MRDV.MetaRawDataVaultTooling.SaveAsync(
            result.Model,
            newWorkspacePathFull,
            cancellationToken).ConfigureAwait(false);

        return result;
    }

    public async Task<MRDV.MetaRawDataVaultModel> MaterializeAsync(
        string sourceWorkspacePath,
        string newWorkspacePath,
        IEnumerable<string>? ignoredFieldNames = null,
        IEnumerable<string>? ignoredFieldSuffixes = null,
        bool includeViews = false,
        CancellationToken cancellationToken = default)
    {
        var result = await MaterializeWithReportAsync(
            sourceWorkspacePath,
            newWorkspacePath,
            ignoredFieldNames,
            ignoredFieldSuffixes,
            includeViews,
            cancellationToken).ConfigureAwait(false);

        return result.Model;
    }

    private static FromMetaSchemaOptions CreateOptions(
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

        return new FromMetaSchemaOptions(ignoredFieldNameSet, ignoredFieldSuffixSet, includeViews);
    }

    private static void EnsureTargetDirectoryIsEmpty(string targetDirectoryPath)
    {
        if (Directory.Exists(targetDirectoryPath) && Directory.EnumerateFileSystemEntries(targetDirectoryPath).Any())
        {
            throw new InvalidOperationException($"target directory '{targetDirectoryPath}' must be empty.");
        }
    }

    public sealed record RawDataVaultFromMetaSchemaResult(
        MRDV.MetaRawDataVaultModel Model,
        RawDataVaultFromMetaSchemaReport Report);
}
