namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLink
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ChildHubId { get; set; } = string.Empty;
        public BusinessHub ChildHub { get; set; } = new BusinessHub();
        public string ParentHubId { get; set; } = string.Empty;
        public BusinessHub ParentHub { get; set; } = new BusinessHub();
    }
}
