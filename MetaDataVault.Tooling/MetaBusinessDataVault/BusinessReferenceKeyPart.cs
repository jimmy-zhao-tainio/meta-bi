namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceKeyPart
    {
        public string Id { get; set; } = string.Empty;
        public string DataTypeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string BusinessReferenceId { get; set; } = string.Empty;
        public BusinessReference BusinessReference { get; set; } = new BusinessReference();
    }
}
