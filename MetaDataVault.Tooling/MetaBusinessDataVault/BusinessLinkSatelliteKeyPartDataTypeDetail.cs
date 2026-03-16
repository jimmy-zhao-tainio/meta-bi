namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkSatelliteKeyPartDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BusinessLinkSatelliteKeyPartId { get; set; } = string.Empty;
        public BusinessLinkSatelliteKeyPart BusinessLinkSatelliteKeyPart { get; set; } = new BusinessLinkSatelliteKeyPart();
    }
}
