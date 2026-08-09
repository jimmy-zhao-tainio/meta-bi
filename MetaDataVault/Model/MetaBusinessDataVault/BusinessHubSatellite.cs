#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessHubSatellite
    {
        public string Id { get; set; } = string.Empty;

        public BusinessHub BusinessHub { get; set; } = null!;

        public BusinessSatellite BusinessSatellite { get; set; } = null!;

    }
}
