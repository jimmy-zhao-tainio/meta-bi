namespace MetaBusinessDataVault
{
    public sealed class BusinessHubSatelliteKeyPartDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string BusinessHubSatelliteKeyPartId { get; internal set; } = string.Empty;
        public BusinessHubSatelliteKeyPart BusinessHubSatelliteKeyPart { get; internal set; } = new BusinessHubSatelliteKeyPart();
    }
}
