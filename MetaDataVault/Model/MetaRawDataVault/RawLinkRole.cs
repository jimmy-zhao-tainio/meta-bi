#nullable enable

namespace MetaRawDataVault
{
    public sealed class RawLinkRole
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public RawHub RawHub { get; set; } = null!;

        public RawLink RawLink { get; set; } = null!;

    }
}
