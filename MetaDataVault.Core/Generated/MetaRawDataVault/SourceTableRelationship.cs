namespace MetaRawDataVault
{
    public sealed class SourceTableRelationship
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string SourceTableId { get; internal set; } = string.Empty;
        public SourceTable SourceTable { get; internal set; } = new SourceTable();
        public string TargetTableId { get; internal set; } = string.Empty;
        public SourceTable TargetTable { get; internal set; } = new SourceTable();
    }
}
