using Meta.Integration;
using Meta.Operations.Domain;
using MetaWeave.Core;
using MetaWeaveScript.Execution;
using MS = global::MetaSchema;
using MRDV = global::MetaRawDataVault;
using Options = global::MetaSchemaToRawDataVaultOptions;

namespace MetaConvert.SchemaToDataVault;

public sealed partial class RawDataVaultFromMetaSchemaService
{
    private static readonly Lazy<MetaWeaveScriptDirection> ForwardDirection = new(
        static () => new MetaWeaveScriptDirectionLoader().Load(
            ResolveWeaveWorkspacePath(),
            "forward"));

    public RawDataVaultFromMetaSchemaResult MaterializeWithReport(
        MS.MetaSchemaModel metaSchemaModel,
        IEnumerable<string>? ignoredFieldNames = null,
        IEnumerable<string>? ignoredFieldSuffixes = null,
        bool includeViews = false)
        => MaterializeWithReport(
            metaSchemaModel,
            ignoredFieldNames,
            ignoredFieldSuffixes,
            includeViews,
            progress: null);

    public RawDataVaultFromMetaSchemaResult MaterializeWithReport(
        MS.MetaSchemaModel metaSchemaModel,
        IEnumerable<string>? ignoredFieldNames,
        IEnumerable<string>? ignoredFieldSuffixes,
        bool includeViews,
        Action<MetaWeaveScriptExecutionProgress>? progress)
    {
        ArgumentNullException.ThrowIfNull(metaSchemaModel);

        var options = CreateOptions(ignoredFieldNames, ignoredFieldSuffixes, includeViews);
        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            ForwardDirection.Value,
            new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
            {
                ["schema"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(metaSchemaModel),
                ["options"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(options.Model),
            },
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(MRDV.MetaRawDataVaultModel.CreateEmpty()),
            stringParameters: null,
            progress);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(
                "The sanctioned MetaSchema-to-Raw-Data-Vault weave rejected the source workspaces:" +
                Environment.NewLine +
                string.Join(
                    Environment.NewLine,
                    result.Issues.Select(static issue => $"{issue.Code}: {issue.Message}")));
        }

        var model = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            MRDV.MetaRawDataVaultModel.CreateEmpty);
        return new RawDataVaultFromMetaSchemaResult(
            model,
            BuildReport(metaSchemaModel, model, options));
    }

    public MRDV.MetaRawDataVaultModel Materialize(
        MS.MetaSchemaModel metaSchemaModel,
        IEnumerable<string>? ignoredFieldNames = null,
        IEnumerable<string>? ignoredFieldSuffixes = null,
        bool includeViews = false)
        => MaterializeWithReport(
            metaSchemaModel,
            ignoredFieldNames,
            ignoredFieldSuffixes,
            includeViews).Model;

    private static OptionsInput CreateOptions(
        IEnumerable<string>? ignoredFieldNames,
        IEnumerable<string>? ignoredFieldSuffixes,
        bool includeViews)
    {
        var names = NormalizeOptionValues(ignoredFieldNames);
        var suffixes = NormalizeOptionValues(ignoredFieldSuffixes);
        var model = Options.MetaSchemaToRawDataVaultOptionsModel.CreateEmpty();
        var root = new Options.ConversionOptions { Id = "options" };
        model.ConversionOptionsList.Add(root);

        for (var index = 0; index < names.Count; index++)
        {
            model.IgnoredFieldNameList.Add(new Options.IgnoredFieldName
            {
                Id = $"ignored-field-name:{index + 1:D8}",
                Value = names[index],
                ConversionOptions = root,
            });
        }

        for (var index = 0; index < suffixes.Count; index++)
        {
            model.IgnoredFieldSuffixList.Add(new Options.IgnoredFieldSuffix
            {
                Id = $"ignored-field-suffix:{index + 1:D8}",
                Value = suffixes[index],
                ConversionOptions = root,
            });
        }

        if (includeViews)
        {
            model.IncludeViewsOptionList.Add(new Options.IncludeViewsOption
            {
                Id = "include-views",
                ConversionOptions = root,
            });
        }

        return new OptionsInput(model, names, suffixes, includeViews);
    }

    private static IReadOnlyList<string> NormalizeOptionValues(IEnumerable<string>? values)
    {
        var result = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var value in values ?? [])
        {
            if (!string.IsNullOrWhiteSpace(value) && seen.Add(value))
            {
                result.Add(value);
            }
        }

        return result;
    }

    private static string ResolveWeaveWorkspacePath()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Weaves",
            "SchemaToRawDataVault");
        if (!File.Exists(Path.Combine(path, "workspace.meta")))
        {
            throw new InvalidOperationException(
                $"The sanctioned MetaSchema-to-Raw-Data-Vault weave was not found at '{path}'.");
        }

        return path;
    }

    private sealed record OptionsInput(
        Options.MetaSchemaToRawDataVaultOptionsModel Model,
        IReadOnlyList<string> IgnoredFieldNames,
        IReadOnlyList<string> IgnoredFieldSuffixes,
        bool IncludeViews);

    public sealed record RawDataVaultFromMetaSchemaResult(
        MRDV.MetaRawDataVaultModel Model,
        RawDataVaultFromMetaSchemaReport Report);
}
