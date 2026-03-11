using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal static class BusinessLinkSatelliteDdlConverter
{
    public static DdlTable Build(BDV.BusinessLinkSatellite satellite, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessLinkSatelliteTable(satellite, context.BusinessDataVault, context.Implementation, context.Conversions);
}
