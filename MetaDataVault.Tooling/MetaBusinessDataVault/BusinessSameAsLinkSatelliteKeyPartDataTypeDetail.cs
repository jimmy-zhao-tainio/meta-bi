namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLinkSatelliteKeyPartDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BusinessSameAsLinkSatelliteKeyPartId { get; set; } = string.Empty;
        public BusinessSameAsLinkSatelliteKeyPart BusinessSameAsLinkSatelliteKeyPart { get; set; } = new BusinessSameAsLinkSatelliteKeyPart();
    }
}
