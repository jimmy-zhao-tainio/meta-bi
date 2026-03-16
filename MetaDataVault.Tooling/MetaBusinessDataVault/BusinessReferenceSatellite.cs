namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceSatellite
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SatelliteKind { get; set; } = string.Empty;
        public string BusinessReferenceId { get; set; } = string.Empty;
        public BusinessReference BusinessReference { get; set; } = new BusinessReference();
    }
}
