#nullable enable

namespace MetaRawDataVault
{
    public sealed class RawHubSatelliteAttribute
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Field Field { get; set; } = null!;

        public RawHubSatellite RawHubSatellite { get; set; } = null!;

    }
}
