namespace MetaBusinessDataVault
{
    public sealed class BusinessHubSatelliteKeyPart
    {
        public string Id { get; internal set; } = string.Empty;
        public string DataTypeId { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string BusinessHubSatelliteId { get; internal set; } = string.Empty;
        public BusinessHubSatellite BusinessHubSatellite { get; internal set; } = new BusinessHubSatellite();
    }
}
