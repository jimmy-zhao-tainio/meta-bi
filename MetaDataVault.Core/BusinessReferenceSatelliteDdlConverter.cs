using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal static class BusinessReferenceSatelliteDdlConverter
{
    public static DdlTable Build(BDV.BusinessReferenceSatellite satellite, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessReferenceSatelliteTable(satellite, context.BusinessDataVault, context.Implementation, context.Conversions);
}
