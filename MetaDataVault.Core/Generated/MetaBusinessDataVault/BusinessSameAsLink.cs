namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLink
    {
        public string Id { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string EquivalentHubId { get; internal set; } = string.Empty;
        public BusinessHub EquivalentHub { get; internal set; } = new BusinessHub();
        public string PrimaryHubId { get; internal set; } = string.Empty;
        public BusinessHub PrimaryHub { get; internal set; } = new BusinessHub();
    }
}
