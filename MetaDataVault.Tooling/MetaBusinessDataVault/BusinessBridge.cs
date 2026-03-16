namespace MetaBusinessDataVault
{
    public sealed class BusinessBridge
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AnchorHubId { get; set; } = string.Empty;
        public BusinessHub AnchorHub { get; set; } = new BusinessHub();
    }
}
