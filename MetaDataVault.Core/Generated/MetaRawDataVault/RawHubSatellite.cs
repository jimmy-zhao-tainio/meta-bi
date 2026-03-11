namespace MetaRawDataVault
{
    public sealed class RawHubSatellite
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string SatelliteKind { get; internal set; } = string.Empty;
        public string RawHubId { get; internal set; } = string.Empty;
        public RawHub RawHub { get; internal set; } = new RawHub();
        public string SourceTableId { get; internal set; } = string.Empty;
        public SourceTable SourceTable { get; internal set; } = new SourceTable();
    }
}
