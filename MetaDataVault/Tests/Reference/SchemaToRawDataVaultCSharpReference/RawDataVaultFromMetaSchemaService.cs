using MS = global::MetaSchema;
using MRDV = global::MetaRawDataVault;

namespace MetaConvert.SchemaToDataVault.Reference;

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

    public sealed record RawDataVaultFromMetaSchemaResult(
        MRDV.MetaRawDataVaultModel Model,
        RawDataVaultFromMetaSchemaReport Report);
}
