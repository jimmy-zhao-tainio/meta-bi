namespace MetaRawDataVault
{
    public sealed class RawHub
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SourceTableId { get; set; } = string.Empty;
        public SourceTable SourceTable { get; set; } = new SourceTable();
    }
}
