#nullable enable

namespace MetaRawDataVault
{
    public sealed class RawHubSatellite
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string SatelliteKind { get; set; } = string.Empty;

        public RawHub RawHub { get; set; } = null!;

    }
}
