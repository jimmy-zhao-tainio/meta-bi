namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceSatelliteKeyPartDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string BusinessReferenceSatelliteKeyPartId { get; internal set; } = string.Empty;
        public BusinessReferenceSatelliteKeyPart BusinessReferenceSatelliteKeyPart { get; internal set; } = new BusinessReferenceSatelliteKeyPart();
    }
}
