namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLinkSatellite
    {
        public string Id { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string SatelliteKind { get; internal set; } = string.Empty;
        public string BusinessSameAsLinkId { get; internal set; } = string.Empty;
        public BusinessSameAsLink BusinessSameAsLink { get; internal set; } = new BusinessSameAsLink();
    }
}
