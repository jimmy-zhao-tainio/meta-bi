#nullable enable

namespace MetaRawDataVault
{
    public sealed class RawLinkSatellite
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string SatelliteKind { get; set; } = string.Empty;

        public RawLink RawLink { get; set; } = null!;

    }
}
