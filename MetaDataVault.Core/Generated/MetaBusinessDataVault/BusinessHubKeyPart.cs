namespace MetaBusinessDataVault
{
    public sealed class BusinessHubKeyPart
    {
        public string Id { get; internal set; } = string.Empty;
        public string DataTypeId { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string BusinessHubId { get; internal set; } = string.Empty;
        public BusinessHub BusinessHub { get; internal set; } = new BusinessHub();
    }
}
