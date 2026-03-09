namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceKeyPartDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string BusinessReferenceKeyPartId { get; internal set; } = string.Empty;
        public BusinessReferenceKeyPart BusinessReferenceKeyPart { get; internal set; } = new BusinessReferenceKeyPart();
    }
}
