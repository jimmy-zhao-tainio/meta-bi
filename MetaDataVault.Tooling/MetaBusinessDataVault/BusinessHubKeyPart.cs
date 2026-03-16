namespace MetaBusinessDataVault
{
    public sealed class BusinessHubKeyPart
    {
        public string Id { get; set; } = string.Empty;
        public string DataTypeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string BusinessHubId { get; set; } = string.Empty;
        public BusinessHub BusinessHub { get; set; } = new BusinessHub();
    }
}
