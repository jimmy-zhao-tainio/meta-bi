namespace MetaRawDataVault
{
    public sealed class SourceTableRelationship
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SourceTableId { get; set; } = string.Empty;
        public SourceTable SourceTable { get; set; } = new SourceTable();
        public string TargetTableId { get; set; } = string.Empty;
        public SourceTable TargetTable { get; set; } = new SourceTable();
    }
}
