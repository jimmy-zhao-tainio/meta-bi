namespace MetaRawDataVault
{
    public sealed class SourceSchema
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string SourceSystemId { get; internal set; } = string.Empty;
        public SourceSystem SourceSystem { get; internal set; } = new SourceSystem();
    }
}
