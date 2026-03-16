namespace MetaRawDataVault
{
    public sealed class RawHubKeyPart
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string RawHubId { get; set; } = string.Empty;
        public RawHub RawHub { get; set; } = new RawHub();
        public string SourceFieldId { get; set; } = string.Empty;
        public SourceField SourceField { get; set; } = new SourceField();
    }
}
