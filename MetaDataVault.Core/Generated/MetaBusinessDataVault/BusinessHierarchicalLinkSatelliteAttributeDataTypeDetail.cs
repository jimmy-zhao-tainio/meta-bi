namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string BusinessHierarchicalLinkSatelliteAttributeId { get; internal set; } = string.Empty;
        public BusinessHierarchicalLinkSatelliteAttribute BusinessHierarchicalLinkSatelliteAttribute { get; internal set; } = new BusinessHierarchicalLinkSatelliteAttribute();
    }
}
