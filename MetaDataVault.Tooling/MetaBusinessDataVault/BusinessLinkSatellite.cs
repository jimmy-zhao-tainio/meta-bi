namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkSatellite
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SatelliteKind { get; set; } = string.Empty;
        public string BusinessLinkId { get; set; } = string.Empty;
        public BusinessLink BusinessLink { get; set; } = new BusinessLink();
    }
}
