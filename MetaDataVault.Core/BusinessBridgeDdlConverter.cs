using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal static class BusinessBridgeDdlConverter
{
    public static DdlTable Build(BDV.BusinessBridge bridge, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessBridgeTable(bridge, context.BusinessDataVault, context.Implementation, context.Conversions);
}
