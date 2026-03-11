namespace MetaRawDataVault
{
    public sealed class RawLinkSatelliteAttribute
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string RawLinkSatelliteId { get; internal set; } = string.Empty;
        public RawLinkSatellite RawLinkSatellite { get; internal set; } = new RawLinkSatellite();
        public string SourceFieldId { get; internal set; } = string.Empty;
        public SourceField SourceField { get; internal set; } = new SourceField();
    }
}
