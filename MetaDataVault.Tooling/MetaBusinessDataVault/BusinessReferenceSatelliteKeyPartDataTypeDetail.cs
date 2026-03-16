namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceSatelliteKeyPartDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BusinessReferenceSatelliteKeyPartId { get; set; } = string.Empty;
        public BusinessReferenceSatelliteKeyPart BusinessReferenceSatelliteKeyPart { get; set; } = new BusinessReferenceSatelliteKeyPart();
    }
}
