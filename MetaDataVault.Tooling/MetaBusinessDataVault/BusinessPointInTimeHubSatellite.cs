namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTimeHubSatellite
    {
        public string Id { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string BusinessHubSatelliteId { get; set; } = string.Empty;
        public BusinessHubSatellite BusinessHubSatellite { get; set; } = new BusinessHubSatellite();
        public string BusinessPointInTimeId { get; set; } = string.Empty;
        public BusinessPointInTime BusinessPointInTime { get; set; } = new BusinessPointInTime();
    }
}
