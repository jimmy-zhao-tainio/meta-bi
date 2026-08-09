#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTimeLinkSatellite
    {
        public string Id { get; set; } = string.Empty;

        public BusinessLinkSatellite BusinessLinkSatellite { get; set; } = null!;

        public BusinessPointInTime BusinessPointInTime { get; set; } = null!;

    }
}
