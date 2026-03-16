namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkHub
    {
        public string Id { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string BusinessHubId { get; set; } = string.Empty;
        public BusinessHub BusinessHub { get; set; } = new BusinessHub();
        public string BusinessLinkId { get; set; } = string.Empty;
        public BusinessLink BusinessLink { get; set; } = new BusinessLink();
    }
}
