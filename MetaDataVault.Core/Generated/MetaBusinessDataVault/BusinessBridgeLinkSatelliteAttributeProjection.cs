namespace MetaBusinessDataVault
{
    public sealed class BusinessBridgeLinkSatelliteAttributeProjection
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string BusinessBridgeId { get; internal set; } = string.Empty;
        public BusinessBridge BusinessBridge { get; internal set; } = new BusinessBridge();
        public string BusinessLinkSatelliteAttributeId { get; internal set; } = string.Empty;
        public BusinessLinkSatelliteAttribute BusinessLinkSatelliteAttribute { get; internal set; } = new BusinessLinkSatelliteAttribute();
    }
}
