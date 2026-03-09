namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceKeyPart
    {
        public string Id { get; internal set; } = string.Empty;
        public string DataTypeId { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string BusinessReferenceId { get; internal set; } = string.Empty;
        public BusinessReference BusinessReference { get; internal set; } = new BusinessReference();
    }
}
