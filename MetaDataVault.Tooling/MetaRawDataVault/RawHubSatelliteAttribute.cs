namespace MetaRawDataVault
{
    public sealed class RawHubSatelliteAttribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string RawHubSatelliteId { get; set; } = string.Empty;
        public RawHubSatellite RawHubSatellite { get; set; } = new RawHubSatellite();
        public string SourceFieldId { get; set; } = string.Empty;
        public SourceField SourceField { get; set; } = new SourceField();
    }
}
