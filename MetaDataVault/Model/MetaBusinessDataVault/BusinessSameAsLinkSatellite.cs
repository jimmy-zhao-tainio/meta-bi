#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLinkSatellite
    {
        public string Id { get; set; } = string.Empty;

        public BusinessSameAsLink BusinessSameAsLink { get; set; } = null!;

        public BusinessSatellite BusinessSatellite { get; set; } = null!;

    }
}
