using MetaBusinessDataVault;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

public static partial class Converter
{
    private static MetaSqlModel ConvertBusiness(MetaBusinessDataVaultModel model, ConversionContext context)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(context);

        var (hubTablesByHubId, hubHashKeyColumnsByHubId) = PopulateBusinessPersistentMetaSqlModel(model, context);
        PopulateBusinessHelperMetaSqlModel(model, context, hubTablesByHubId, hubHashKeyColumnsByHubId);

        return context.MetaSql;
    }
}
