namespace MetaBusinessDataVault
{
    public sealed class BusinessBridgeLinkSatelliteAttributeProjection
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string BusinessBridgeId { get; set; } = string.Empty;
        public BusinessBridge BusinessBridge { get; set; } = new BusinessBridge();
        public string BusinessLinkSatelliteAttributeId { get; set; } = string.Empty;
        public BusinessLinkSatelliteAttribute BusinessLinkSatelliteAttribute { get; set; } = new BusinessLinkSatelliteAttribute();
    }
}
