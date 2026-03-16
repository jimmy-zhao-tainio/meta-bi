namespace MetaRawDataVault
{
    public sealed class RawLinkHub
    {
        public string Id { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string RawHubId { get; set; } = string.Empty;
        public RawHub RawHub { get; set; } = new RawHub();
        public string RawLinkId { get; set; } = string.Empty;
        public RawLink RawLink { get; set; } = new RawLink();
    }
}
