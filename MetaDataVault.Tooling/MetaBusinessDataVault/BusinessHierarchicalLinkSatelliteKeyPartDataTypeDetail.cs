namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLinkSatelliteKeyPartDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BusinessHierarchicalLinkSatelliteKeyPartId { get; set; } = string.Empty;
        public BusinessHierarchicalLinkSatelliteKeyPart BusinessHierarchicalLinkSatelliteKeyPart { get; set; } = new BusinessHierarchicalLinkSatelliteKeyPart();
    }
}
