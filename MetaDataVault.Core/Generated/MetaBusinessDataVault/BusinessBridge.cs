namespace MetaBusinessDataVault
{
    public sealed class BusinessBridge
    {
        public string Id { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string AnchorHubId { get; internal set; } = string.Empty;
        public BusinessHub AnchorHub { get; internal set; } = new BusinessHub();
    }
}
