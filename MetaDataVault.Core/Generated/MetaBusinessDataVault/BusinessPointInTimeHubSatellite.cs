namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTimeHubSatellite
    {
        public string Id { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string BusinessHubSatelliteId { get; internal set; } = string.Empty;
        public BusinessHubSatellite BusinessHubSatellite { get; internal set; } = new BusinessHubSatellite();
        public string BusinessPointInTimeId { get; internal set; } = string.Empty;
        public BusinessPointInTime BusinessPointInTime { get; internal set; } = new BusinessPointInTime();
    }
}
