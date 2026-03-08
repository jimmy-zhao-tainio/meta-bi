namespace MetaBusinessDataVault
{
    public sealed class BusinessBridgeLink
    {
        public string Id { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string RoleName { get; internal set; } = string.Empty;
        public string BusinessBridgeId { get; internal set; } = string.Empty;
        public BusinessBridge BusinessBridge { get; internal set; } = new BusinessBridge();
        public string BusinessLinkId { get; internal set; } = string.Empty;
        public BusinessLink BusinessLink { get; internal set; } = new BusinessLink();
    }
}
