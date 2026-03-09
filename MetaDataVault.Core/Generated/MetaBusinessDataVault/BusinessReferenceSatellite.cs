namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceSatellite
    {
        public string Id { get; internal set; } = string.Empty;
        public string Description { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string SatelliteKind { get; internal set; } = string.Empty;
        public string BusinessReferenceId { get; internal set; } = string.Empty;
        public BusinessReference BusinessReference { get; internal set; } = new BusinessReference();
    }
}
