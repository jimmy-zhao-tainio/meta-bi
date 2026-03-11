using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal static class BusinessHubDdlConverter
{
    public static DdlTable Build(BDV.BusinessHub hub, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessHubTable(hub, context.BusinessDataVault, context.Implementation, context.Conversions);
}
