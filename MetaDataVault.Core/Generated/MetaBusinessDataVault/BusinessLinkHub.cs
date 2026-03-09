namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkHub
    {
        public string Id { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string RoleName { get; internal set; } = string.Empty;
        public string BusinessHubId { get; internal set; } = string.Empty;
        public BusinessHub BusinessHub { get; internal set; } = new BusinessHub();
        public string BusinessLinkId { get; internal set; } = string.Empty;
        public BusinessLink BusinessLink { get; internal set; } = new BusinessLink();
    }
}
