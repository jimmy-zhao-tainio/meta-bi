namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceSatelliteKeyPart
    {
        public string Id { get; set; } = string.Empty;
        public string DataTypeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string BusinessReferenceSatelliteId { get; set; } = string.Empty;
        public BusinessReferenceSatellite BusinessReferenceSatellite { get; set; } = new BusinessReferenceSatellite();
    }
}
