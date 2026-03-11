using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal static class BusinessReferenceDdlConverter
{
    public static DdlTable Build(BDV.BusinessReference reference, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessReferenceTable(reference, context.BusinessDataVault, context.Implementation, context.Conversions);
}
