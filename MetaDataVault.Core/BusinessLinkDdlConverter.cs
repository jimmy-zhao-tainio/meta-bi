using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal static class BusinessLinkDdlConverter
{
    public static DdlTable Build(BDV.BusinessLink link, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessLinkTable(link, context.BusinessDataVault, context.Implementation, context.Conversions);
}
