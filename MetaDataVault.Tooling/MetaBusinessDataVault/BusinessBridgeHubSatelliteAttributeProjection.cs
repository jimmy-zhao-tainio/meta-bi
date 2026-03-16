namespace MetaBusinessDataVault
{
    public sealed class BusinessBridgeHubSatelliteAttributeProjection
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string BusinessBridgeId { get; set; } = string.Empty;
        public BusinessBridge BusinessBridge { get; set; } = new BusinessBridge();
        public string BusinessHubSatelliteAttributeId { get; set; } = string.Empty;
        public BusinessHubSatelliteAttribute BusinessHubSatelliteAttribute { get; set; } = new BusinessHubSatelliteAttribute();
    }
}
