namespace MetaRawDataVault
{
    public sealed class SourceTableRelationship
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string TargetSchemaName { get; internal set; } = string.Empty;
        public string TargetTableName { get; internal set; } = string.Empty;
        public string SourceTableId { get; internal set; } = string.Empty;
        public SourceTable SourceTable { get; internal set; } = new SourceTable();
    }
}
