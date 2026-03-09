namespace MetaBusinessDataVault
{
    public sealed class BusinessBridgeHubSatelliteAttributeProjection
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string BusinessBridgeId { get; internal set; } = string.Empty;
        public BusinessBridge BusinessBridge { get; internal set; } = new BusinessBridge();
        public string BusinessHubSatelliteAttributeId { get; internal set; } = string.Empty;
        public BusinessHubSatelliteAttribute BusinessHubSatelliteAttribute { get; internal set; } = new BusinessHubSatelliteAttribute();
    }
}
