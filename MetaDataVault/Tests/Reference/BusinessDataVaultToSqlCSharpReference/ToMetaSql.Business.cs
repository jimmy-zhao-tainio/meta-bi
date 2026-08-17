using MetaBusinessDataVault;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

internal static partial class BusinessDataVaultToSqlCSharpReference
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
