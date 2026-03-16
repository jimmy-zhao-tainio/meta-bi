namespace MetaBusinessDataVault
{
    public sealed class BusinessHubSatelliteKeyPartDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BusinessHubSatelliteKeyPartId { get; set; } = string.Empty;
        public BusinessHubSatelliteKeyPart BusinessHubSatelliteKeyPart { get; set; } = new BusinessHubSatelliteKeyPart();
    }
}
