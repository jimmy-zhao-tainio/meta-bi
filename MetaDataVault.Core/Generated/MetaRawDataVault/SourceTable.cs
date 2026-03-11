namespace MetaRawDataVault
{
    public sealed class SourceTable
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string SourceSchemaId { get; internal set; } = string.Empty;
        public SourceSchema SourceSchema { get; internal set; } = new SourceSchema();
    }
}
