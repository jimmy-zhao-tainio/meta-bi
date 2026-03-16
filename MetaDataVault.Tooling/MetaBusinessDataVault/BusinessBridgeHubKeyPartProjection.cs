namespace MetaBusinessDataVault
{
    public sealed class BusinessBridgeHubKeyPartProjection
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string BusinessBridgeId { get; set; } = string.Empty;
        public BusinessBridge BusinessBridge { get; set; } = new BusinessBridge();
        public string BusinessHubKeyPartId { get; set; } = string.Empty;
        public BusinessHubKeyPart BusinessHubKeyPart { get; set; } = new BusinessHubKeyPart();
    }
}
