namespace MetaBusinessDataVault
{
    public sealed class BusinessBridgeHubKeyPartProjection
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string BusinessBridgeId { get; internal set; } = string.Empty;
        public BusinessBridge BusinessBridge { get; internal set; } = new BusinessBridge();
        public string BusinessHubKeyPartId { get; internal set; } = string.Empty;
        public BusinessHubKeyPart BusinessHubKeyPart { get; internal set; } = new BusinessHubKeyPart();
    }
}
