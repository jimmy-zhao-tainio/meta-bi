#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceSatellite
    {
        public string Id { get; set; } = string.Empty;

        public BusinessReference BusinessReference { get; set; } = null!;

        public BusinessSatellite BusinessSatellite { get; set; } = null!;

    }
}
