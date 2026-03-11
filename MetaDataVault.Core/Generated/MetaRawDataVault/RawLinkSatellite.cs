namespace MetaRawDataVault
{
    public sealed class RawLinkSatellite
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string SatelliteKind { get; internal set; } = string.Empty;
        public string RawLinkId { get; internal set; } = string.Empty;
        public RawLink RawLink { get; internal set; } = new RawLink();
        public string SourceTableId { get; internal set; } = string.Empty;
        public SourceTable SourceTable { get; internal set; } = new SourceTable();
    }
}
