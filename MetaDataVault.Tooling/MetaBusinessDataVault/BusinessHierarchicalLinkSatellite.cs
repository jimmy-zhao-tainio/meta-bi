namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLinkSatellite
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SatelliteKind { get; set; } = string.Empty;
        public string BusinessHierarchicalLinkId { get; set; } = string.Empty;
        public BusinessHierarchicalLink BusinessHierarchicalLink { get; set; } = new BusinessHierarchicalLink();
    }
}
