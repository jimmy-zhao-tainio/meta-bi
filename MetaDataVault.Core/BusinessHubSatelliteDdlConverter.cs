using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal static class BusinessHubSatelliteDdlConverter
{
    public static DdlTable Build(BDV.BusinessHubSatellite satellite, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessHubSatelliteTable(satellite, context.BusinessDataVault, context.Implementation, context.Conversions);
}
