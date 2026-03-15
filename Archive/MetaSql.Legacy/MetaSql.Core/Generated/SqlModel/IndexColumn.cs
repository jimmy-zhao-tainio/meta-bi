namespace SqlModel
{
    public sealed class IndexColumn
    {
        public string Id { get; internal set; } = string.Empty;
        public string IsDescending { get; internal set; } = string.Empty;
        public string IsIncluded { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string IndexId { get; internal set; } = string.Empty;
        public Index Index { get; internal set; } = new Index();
        public string TableColumnId { get; internal set; } = string.Empty;
        public TableColumn TableColumn { get; internal set; } = new TableColumn();
    }
}
