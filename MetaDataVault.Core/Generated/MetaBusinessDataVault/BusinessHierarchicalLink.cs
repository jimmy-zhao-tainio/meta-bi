namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLink
    {
        public string Id { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string ChildHubId { get; internal set; } = string.Empty;
        public BusinessHub ChildHub { get; internal set; } = new BusinessHub();
        public string ParentHubId { get; internal set; } = string.Empty;
        public BusinessHub ParentHub { get; internal set; } = new BusinessHub();
    }
}
