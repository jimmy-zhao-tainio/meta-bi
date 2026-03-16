namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLink
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string EquivalentHubId { get; set; } = string.Empty;
        public BusinessHub EquivalentHub { get; set; } = new BusinessHub();
        public string PrimaryHubId { get; set; } = string.Empty;
        public BusinessHub PrimaryHub { get; set; } = new BusinessHub();
    }
}
