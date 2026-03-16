namespace MetaSchema
{
    public sealed class TableRelationship
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SourceTableId { get; set; } = string.Empty;
        public Table SourceTable { get; set; } = new Table();
        public string TargetTableId { get; set; } = string.Empty;
        public Table TargetTable { get; set; } = new Table();
    }
}
