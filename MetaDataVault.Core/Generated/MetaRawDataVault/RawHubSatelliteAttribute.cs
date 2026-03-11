namespace MetaRawDataVault
{
    public sealed class RawHubSatelliteAttribute
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string RawHubSatelliteId { get; internal set; } = string.Empty;
        public RawHubSatellite RawHubSatellite { get; internal set; } = new RawHubSatellite();
        public string SourceFieldId { get; internal set; } = string.Empty;
        public SourceField SourceField { get; internal set; } = new SourceField();
    }
}
