namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceSatelliteAttributeDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string BusinessReferenceSatelliteAttributeId { get; internal set; } = string.Empty;
        public BusinessReferenceSatelliteAttribute BusinessReferenceSatelliteAttribute { get; internal set; } = new BusinessReferenceSatelliteAttribute();
    }
}
