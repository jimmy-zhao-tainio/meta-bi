namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLinkSatelliteAttribute
    {
        public string Id { get; internal set; } = string.Empty;
        public string DataTypeId { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string BusinessHierarchicalLinkSatelliteId { get; internal set; } = string.Empty;
        public BusinessHierarchicalLinkSatellite BusinessHierarchicalLinkSatellite { get; internal set; } = new BusinessHierarchicalLinkSatellite();
    }
}
