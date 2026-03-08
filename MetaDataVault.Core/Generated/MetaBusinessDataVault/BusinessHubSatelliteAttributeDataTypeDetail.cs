namespace MetaBusinessDataVault
{
    public sealed class BusinessHubSatelliteAttributeDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string BusinessHubSatelliteAttributeId { get; internal set; } = string.Empty;
        public BusinessHubSatelliteAttribute BusinessHubSatelliteAttribute { get; internal set; } = new BusinessHubSatelliteAttribute();
    }
}
