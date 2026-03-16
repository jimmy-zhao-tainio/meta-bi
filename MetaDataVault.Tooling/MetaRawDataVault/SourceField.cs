namespace MetaRawDataVault
{
    public sealed class SourceField
    {
        public string Id { get; set; } = string.Empty;
        public string DataTypeId { get; set; } = string.Empty;
        public string IsNullable { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string SourceTableId { get; set; } = string.Empty;
        public SourceTable SourceTable { get; set; } = new SourceTable();
    }
}
