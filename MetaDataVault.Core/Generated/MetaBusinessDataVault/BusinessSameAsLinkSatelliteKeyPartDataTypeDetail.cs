namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLinkSatelliteKeyPartDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string BusinessSameAsLinkSatelliteKeyPartId { get; internal set; } = string.Empty;
        public BusinessSameAsLinkSatelliteKeyPart BusinessSameAsLinkSatelliteKeyPart { get; internal set; } = new BusinessSameAsLinkSatelliteKeyPart();
    }
}
