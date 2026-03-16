namespace MetaBusinessDataVault
{
    public sealed class BusinessHubKeyPartDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BusinessHubKeyPartId { get; set; } = string.Empty;
        public BusinessHubKeyPart BusinessHubKeyPart { get; set; } = new BusinessHubKeyPart();
    }
}
