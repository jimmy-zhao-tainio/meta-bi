namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLinkSatelliteKeyPart
    {
        public string Id { get; internal set; } = string.Empty;
        public string DataTypeId { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string BusinessSameAsLinkSatelliteId { get; internal set; } = string.Empty;
        public BusinessSameAsLinkSatellite BusinessSameAsLinkSatellite { get; internal set; } = new BusinessSameAsLinkSatellite();
    }
}
