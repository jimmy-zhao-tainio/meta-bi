using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal static class BusinessSameAsLinkSatelliteDdlConverter
{
    public static DdlTable Build(BDV.BusinessSameAsLinkSatellite satellite, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessSameAsLinkSatelliteTable(satellite, context.BusinessDataVault, context.Implementation, context.Conversions);
}
