namespace SqlModel
{
    public sealed class ForeignKeyColumn
    {
        public string Id { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string ForeignKeyId { get; internal set; } = string.Empty;
        public ForeignKey ForeignKey { get; internal set; } = new ForeignKey();
        public string SourceColumnId { get; internal set; } = string.Empty;
        public TableColumn SourceColumn { get; internal set; } = new TableColumn();
        public string TargetColumnId { get; internal set; } = string.Empty;
        public TableColumn TargetColumn { get; internal set; } = new TableColumn();
    }
}
