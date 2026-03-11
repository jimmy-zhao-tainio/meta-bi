using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal static class BusinessSameAsLinkDdlConverter
{
    public static DdlTable Build(BDV.BusinessSameAsLink link, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessSameAsLinkTable(link, context.Implementation, context.Conversions);
}
