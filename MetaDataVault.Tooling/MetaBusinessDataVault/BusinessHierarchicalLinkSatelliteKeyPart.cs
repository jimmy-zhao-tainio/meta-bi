namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLinkSatelliteKeyPart
    {
        public string Id { get; set; } = string.Empty;
        public string DataTypeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string BusinessHierarchicalLinkSatelliteId { get; set; } = string.Empty;
        public BusinessHierarchicalLinkSatellite BusinessHierarchicalLinkSatellite { get; set; } = new BusinessHierarchicalLinkSatellite();
    }
}
