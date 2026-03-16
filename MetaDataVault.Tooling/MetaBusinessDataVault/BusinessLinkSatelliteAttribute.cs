namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkSatelliteAttribute
    {
        public string Id { get; set; } = string.Empty;
        public string DataTypeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string BusinessLinkSatelliteId { get; set; } = string.Empty;
        public BusinessLinkSatellite BusinessLinkSatellite { get; set; } = new BusinessLinkSatellite();
    }
}
