namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLinkSatelliteKeyPartDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string BusinessHierarchicalLinkSatelliteKeyPartId { get; internal set; } = string.Empty;
        public BusinessHierarchicalLinkSatelliteKeyPart BusinessHierarchicalLinkSatelliteKeyPart { get; internal set; } = new BusinessHierarchicalLinkSatelliteKeyPart();
    }
}
