#nullable enable

namespace MetaRawDataVault
{
    public sealed class RawLinkSatelliteAttribute
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Field Field { get; set; } = null!;

        public RawLinkSatellite RawLinkSatellite { get; set; } = null!;

    }
}
