using Meta.Core.Domain;
using MetaBusinessDataVault;
using MetaSql;

namespace MetaDataVault.ToMetaSql;

public static partial class Converter
{
    private static Workspace ConvertBusiness(MetaBusinessDataVaultModel model, ConversionContext context)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(context);

        var (hubTablesByHubId, hubHashKeyColumnsByHubId) = PopulateBusinessPersistentMetaSqlModel(model, context);
        PopulateBusinessHelperMetaSqlModel(model, context, hubTablesByHubId, hubHashKeyColumnsByHubId);

        return context.MetaSql.ToXmlWorkspace(context.PathToNewMetaSqlWorkspace);
    }
}
