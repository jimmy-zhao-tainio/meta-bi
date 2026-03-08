namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkSatelliteKeyPart
    {
        public string Id { get; internal set; } = string.Empty;
        public string DataTypeId { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string BusinessLinkSatelliteId { get; internal set; } = string.Empty;
        public BusinessLinkSatellite BusinessLinkSatellite { get; internal set; } = new BusinessLinkSatellite();
    }
}
