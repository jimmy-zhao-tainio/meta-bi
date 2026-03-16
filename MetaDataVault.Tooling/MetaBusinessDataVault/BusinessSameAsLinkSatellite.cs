namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLinkSatellite
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SatelliteKind { get; set; } = string.Empty;
        public string BusinessSameAsLinkId { get; set; } = string.Empty;
        public BusinessSameAsLink BusinessSameAsLink { get; set; } = new BusinessSameAsLink();
    }
}
