namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BusinessHierarchicalLinkSatelliteAttributeId { get; set; } = string.Empty;
        public BusinessHierarchicalLinkSatelliteAttribute BusinessHierarchicalLinkSatelliteAttribute { get; set; } = new BusinessHierarchicalLinkSatelliteAttribute();
    }
}
