namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkSatelliteAttributeDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string BusinessLinkSatelliteAttributeId { get; internal set; } = string.Empty;
        public BusinessLinkSatelliteAttribute BusinessLinkSatelliteAttribute { get; internal set; } = new BusinessLinkSatelliteAttribute();
    }
}
