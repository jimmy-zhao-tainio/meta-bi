namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkSatelliteKeyPartDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string BusinessLinkSatelliteKeyPartId { get; internal set; } = string.Empty;
        public BusinessLinkSatelliteKeyPart BusinessLinkSatelliteKeyPart { get; internal set; } = new BusinessLinkSatelliteKeyPart();
    }
}
