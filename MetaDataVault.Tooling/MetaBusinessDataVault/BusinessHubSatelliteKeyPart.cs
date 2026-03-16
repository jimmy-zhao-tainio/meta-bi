namespace MetaBusinessDataVault
{
    public sealed class BusinessHubSatelliteKeyPart
    {
        public string Id { get; set; } = string.Empty;
        public string DataTypeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string BusinessHubSatelliteId { get; set; } = string.Empty;
        public BusinessHubSatellite BusinessHubSatellite { get; set; } = new BusinessHubSatellite();
    }
}
