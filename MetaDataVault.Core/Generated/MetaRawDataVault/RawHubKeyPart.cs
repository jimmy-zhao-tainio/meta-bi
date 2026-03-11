namespace MetaRawDataVault
{
    public sealed class RawHubKeyPart
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string RawHubId { get; internal set; } = string.Empty;
        public RawHub RawHub { get; internal set; } = new RawHub();
        public string SourceFieldId { get; internal set; } = string.Empty;
        public SourceField SourceField { get; internal set; } = new SourceField();
    }
}
