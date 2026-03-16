namespace MetaRawDataVault
{
    public sealed class SourceTable
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SourceSchemaId { get; set; } = string.Empty;
        public SourceSchema SourceSchema { get; set; } = new SourceSchema();
    }
}
