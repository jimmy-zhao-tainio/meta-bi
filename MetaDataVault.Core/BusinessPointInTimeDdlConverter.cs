using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal static class BusinessPointInTimeDdlConverter
{
    public static DdlTable Build(BDV.BusinessPointInTime pointInTime, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessPointInTimeTable(pointInTime, context.BusinessDataVault, context.Implementation, context.Conversions);
}
