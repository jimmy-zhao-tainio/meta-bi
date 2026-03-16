namespace MetaBusinessDataVault
{
    public sealed class BusinessHubSatelliteAttributeDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BusinessHubSatelliteAttributeId { get; set; } = string.Empty;
        public BusinessHubSatelliteAttribute BusinessHubSatelliteAttribute { get; set; } = new BusinessHubSatelliteAttribute();
    }
}
