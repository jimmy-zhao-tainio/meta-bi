namespace MetaSql
{
    public sealed class ForeignKeyColumn
    {
        public string Id { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string ForeignKeyId { get; set; } = string.Empty;
        public ForeignKey ForeignKey { get; set; } = new ForeignKey();
        public string SourceColumnId { get; set; } = string.Empty;
        public TableColumn SourceColumn { get; set; } = new TableColumn();
        public string TargetColumnId { get; set; } = string.Empty;
        public TableColumn TargetColumn { get; set; } = new TableColumn();
    }
}
