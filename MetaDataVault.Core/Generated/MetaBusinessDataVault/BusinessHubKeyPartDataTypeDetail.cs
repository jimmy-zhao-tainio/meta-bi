namespace MetaBusinessDataVault
{
    public sealed class BusinessHubKeyPartDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string BusinessHubKeyPartId { get; internal set; } = string.Empty;
        public BusinessHubKeyPart BusinessHubKeyPart { get; internal set; } = new BusinessHubKeyPart();
    }
}
