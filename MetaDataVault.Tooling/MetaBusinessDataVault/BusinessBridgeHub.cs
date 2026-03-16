namespace MetaBusinessDataVault
{
    public sealed class BusinessBridgeHub
    {
        public string Id { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string BusinessBridgeId { get; set; } = string.Empty;
        public BusinessBridge BusinessBridge { get; set; } = new BusinessBridge();
        public string BusinessHubId { get; set; } = string.Empty;
        public BusinessHub BusinessHub { get; set; } = new BusinessHub();
    }
}
