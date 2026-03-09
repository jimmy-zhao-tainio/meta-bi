namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceSatelliteAttribute
    {
        public string Id { get; internal set; } = string.Empty;
        public string DataTypeId { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string BusinessReferenceSatelliteId { get; internal set; } = string.Empty;
        public BusinessReferenceSatellite BusinessReferenceSatellite { get; internal set; } = new BusinessReferenceSatellite();
    }
}
