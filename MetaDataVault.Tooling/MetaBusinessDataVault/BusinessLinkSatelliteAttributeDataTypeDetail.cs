namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkSatelliteAttributeDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BusinessLinkSatelliteAttributeId { get; set; } = string.Empty;
        public BusinessLinkSatelliteAttribute BusinessLinkSatelliteAttribute { get; set; } = new BusinessLinkSatelliteAttribute();
    }
}
