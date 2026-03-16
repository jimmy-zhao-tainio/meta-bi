namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLinkSatelliteAttribute
    {
        public string Id { get; set; } = string.Empty;
        public string DataTypeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string BusinessSameAsLinkSatelliteId { get; set; } = string.Empty;
        public BusinessSameAsLinkSatellite BusinessSameAsLinkSatellite { get; set; } = new BusinessSameAsLinkSatellite();
    }
}
