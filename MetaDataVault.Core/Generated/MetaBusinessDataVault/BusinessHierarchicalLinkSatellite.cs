namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLinkSatellite
    {
        public string Id { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string SatelliteKind { get; internal set; } = string.Empty;
        public string BusinessHierarchicalLinkId { get; internal set; } = string.Empty;
        public BusinessHierarchicalLink BusinessHierarchicalLink { get; internal set; } = new BusinessHierarchicalLink();
    }
}
