namespace MetaBusinessDataVault
{
    public sealed class BusinessPointInTimeLinkSatellite
    {
        public string Id { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string BusinessLinkSatelliteId { get; internal set; } = string.Empty;
        public BusinessLinkSatellite BusinessLinkSatellite { get; internal set; } = new BusinessLinkSatellite();
        public string BusinessPointInTimeId { get; internal set; } = string.Empty;
        public BusinessPointInTime BusinessPointInTime { get; internal set; } = new BusinessPointInTime();
    }
}
