namespace MetaBusinessDataVault
{
    public sealed class BusinessBridgeHub
    {
        public string Id { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string RoleName { get; internal set; } = string.Empty;
        public string BusinessBridgeId { get; internal set; } = string.Empty;
        public BusinessBridge BusinessBridge { get; internal set; } = new BusinessBridge();
        public string BusinessHubId { get; internal set; } = string.Empty;
        public BusinessHub BusinessHub { get; internal set; } = new BusinessHub();
    }
}
