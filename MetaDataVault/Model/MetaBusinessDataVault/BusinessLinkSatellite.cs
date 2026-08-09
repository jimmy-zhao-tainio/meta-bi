#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkSatellite
    {
        public string Id { get; set; } = string.Empty;

        public BusinessLink BusinessLink { get; set; } = null!;

        public BusinessSatellite BusinessSatellite { get; set; } = null!;

    }
}
