using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal static class BusinessDataVaultSqlObjectExtensions
{
    public static DdlTable ToDdl(this BDV.BusinessHub hub, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessHubTable(hub, context.BusinessDataVault, context.Implementation, context.Conversions);

    public static DdlTable ToDdl(this BDV.BusinessLink link, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessLinkTable(link, context.BusinessDataVault, context.Implementation, context.Conversions);

    public static DdlTable ToDdl(this BDV.BusinessSameAsLink link, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessSameAsLinkTable(link, context.Implementation, context.Conversions);

    public static DdlTable ToDdl(this BDV.BusinessHierarchicalLink link, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessHierarchicalLinkTable(link, context.Implementation, context.Conversions);

    public static DdlTable ToDdl(this BDV.BusinessReference reference, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessReferenceTable(reference, context.BusinessDataVault, context.Implementation, context.Conversions);

    public static DdlTable ToDdl(this BDV.BusinessHubSatellite satellite, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessHubSatelliteTable(satellite, context.BusinessDataVault, context.Implementation, context.Conversions);

    public static DdlTable ToDdl(this BDV.BusinessLinkSatellite satellite, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessLinkSatelliteTable(satellite, context.BusinessDataVault, context.Implementation, context.Conversions);

    public static DdlTable ToDdl(this BDV.BusinessSameAsLinkSatellite satellite, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessSameAsLinkSatelliteTable(satellite, context.BusinessDataVault, context.Implementation, context.Conversions);

    public static DdlTable ToDdl(this BDV.BusinessHierarchicalLinkSatellite satellite, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessHierarchicalLinkSatelliteTable(satellite, context.BusinessDataVault, context.Implementation, context.Conversions);

    public static DdlTable ToDdl(this BDV.BusinessReferenceSatellite satellite, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessReferenceSatelliteTable(satellite, context.BusinessDataVault, context.Implementation, context.Conversions);

    public static DdlTable ToDdl(this BDV.BusinessPointInTime pointInTime, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessPointInTimeTable(pointInTime, context.BusinessDataVault, context.Implementation, context.Conversions);

    public static DdlTable ToDdl(this BDV.BusinessBridge bridge, BusinessDataVaultSqlGenerationContext context)
        => BusinessDataVaultSqlGenerator.BuildBusinessBridgeTable(bridge, context.BusinessDataVault, context.Implementation, context.Conversions);
}
