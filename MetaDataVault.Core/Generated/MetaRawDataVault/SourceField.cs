namespace MetaRawDataVault
{
    public sealed class SourceField
    {
        public string Id { get; internal set; } = string.Empty;
        public string DataTypeId { get; internal set; } = string.Empty;
        public string IsNullable { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string SourceTableId { get; internal set; } = string.Empty;
        public SourceTable SourceTable { get; internal set; } = new SourceTable();
    }
}
