namespace MetaRawDataVault
{
    public sealed class RawLinkHub
    {
        public string Id { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string RoleName { get; internal set; } = string.Empty;
        public string RawHubId { get; internal set; } = string.Empty;
        public RawHub RawHub { get; internal set; } = new RawHub();
        public string RawLinkId { get; internal set; } = string.Empty;
        public RawLink RawLink { get; internal set; } = new RawLink();
    }
}
