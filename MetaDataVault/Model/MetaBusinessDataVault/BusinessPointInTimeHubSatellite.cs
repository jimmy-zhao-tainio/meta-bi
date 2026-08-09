#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTimeHubSatellite
    {
        public string Id { get; set; } = string.Empty;

        public BusinessHubSatellite BusinessHubSatellite { get; set; } = null!;

        public BusinessPointInTime BusinessPointInTime { get; set; } = null!;

    }
}
