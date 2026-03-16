namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLinkSatelliteAttributeDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BusinessSameAsLinkSatelliteAttributeId { get; set; } = string.Empty;
        public BusinessSameAsLinkSatelliteAttribute BusinessSameAsLinkSatelliteAttribute { get; set; } = new BusinessSameAsLinkSatelliteAttribute();
    }
}
