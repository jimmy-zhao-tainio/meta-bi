namespace MetaRawDataVault
{
    public sealed class RawLinkSatellite
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SatelliteKind { get; set; } = string.Empty;
        public string RawLinkId { get; set; } = string.Empty;
        public RawLink RawLink { get; set; } = new RawLink();
        public string SourceTableId { get; set; } = string.Empty;
        public SourceTable SourceTable { get; set; } = new SourceTable();
    }
}
