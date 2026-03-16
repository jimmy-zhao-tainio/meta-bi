namespace MetaRawDataVault
{
    public sealed class RawLinkSatelliteAttribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string RawLinkSatelliteId { get; set; } = string.Empty;
        public RawLinkSatellite RawLinkSatellite { get; set; } = new RawLinkSatellite();
        public string SourceFieldId { get; set; } = string.Empty;
        public SourceField SourceField { get; set; } = new SourceField();
    }
}
