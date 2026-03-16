namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceSatelliteAttributeDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BusinessReferenceSatelliteAttributeId { get; set; } = string.Empty;
        public BusinessReferenceSatelliteAttribute BusinessReferenceSatelliteAttribute { get; set; } = new BusinessReferenceSatelliteAttribute();
    }
}
