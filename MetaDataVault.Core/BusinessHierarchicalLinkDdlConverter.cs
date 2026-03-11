using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal static class BusinessHierarchicalLinkDdlConverter
{
    public static DdlTable Build(BDV.BusinessHierarchicalLink link, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessHierarchicalLinkTable(link, context.Implementation, context.Conversions);
}
