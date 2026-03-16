namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTimeLinkSatellite
    {
        public string Id { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string BusinessLinkSatelliteId { get; set; } = string.Empty;
        public BusinessLinkSatellite BusinessLinkSatellite { get; set; } = new BusinessLinkSatellite();
        public string BusinessPointInTimeId { get; set; } = string.Empty;
        public BusinessPointInTime BusinessPointInTime { get; set; } = new BusinessPointInTime();
    }
}
