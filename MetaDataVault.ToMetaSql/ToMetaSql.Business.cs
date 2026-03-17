using Meta.Core.Domain;
using MetaBusinessDataVault;
using SqlModel;

namespace MetaDataVault.ToMetaSql;

public static partial class Converter
{
    private static Workspace ConvertBusiness(MetaBusinessDataVaultModel model, ConversionContext context)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(context);

        var (hubTablesByHubId, hubHashKeyColumnsByHubId) = PopulateBusinessPersistentSqlModel(model, context);
        PopulateBusinessHelperSqlModel(model, context, hubTablesByHubId, hubHashKeyColumnsByHubId);

        return context.SqlModel.ToXmlWorkspace(context.PathToNewMetaSqlWorkspace);
    }
}
