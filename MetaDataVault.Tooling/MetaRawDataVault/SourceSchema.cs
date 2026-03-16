namespace MetaRawDataVault
{
    public sealed class SourceSchema
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SourceSystemId { get; set; } = string.Empty;
        public SourceSystem SourceSystem { get; set; } = new SourceSystem();
    }
}
