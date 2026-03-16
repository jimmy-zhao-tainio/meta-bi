namespace MetaBusinessDataVault
{
    public sealed class BusinessBridgeLink
    {
        public string Id { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string BusinessBridgeId { get; set; } = string.Empty;
        public BusinessBridge BusinessBridge { get; set; } = new BusinessBridge();
        public string BusinessLinkId { get; set; } = string.Empty;
        public BusinessLink BusinessLink { get; set; } = new BusinessLink();
    }
}
