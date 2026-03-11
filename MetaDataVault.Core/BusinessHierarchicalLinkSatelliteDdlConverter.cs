using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal static class BusinessHierarchicalLinkSatelliteDdlConverter
{
    public static DdlTable Build(BDV.BusinessHierarchicalLinkSatellite satellite, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessHierarchicalLinkSatelliteTable(satellite, context.BusinessDataVault, context.Implementation, context.Conversions);
}
