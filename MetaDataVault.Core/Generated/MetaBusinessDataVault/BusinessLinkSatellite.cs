namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkSatellite
    {
        public string Id { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string SatelliteKind { get; internal set; } = string.Empty;
        public string BusinessLinkId { get; internal set; } = string.Empty;
        public BusinessLink BusinessLink { get; internal set; } = new BusinessLink();
    }
}
