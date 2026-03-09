namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLinkSatelliteAttributeDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string BusinessSameAsLinkSatelliteAttributeId { get; internal set; } = string.Empty;
        public BusinessSameAsLinkSatelliteAttribute BusinessSameAsLinkSatelliteAttribute { get; internal set; } = new BusinessSameAsLinkSatelliteAttribute();
    }
}
