namespace SqlModel
{
    public sealed class IndexColumn
    {
        public string Id { get; set; } = string.Empty;
        public string IsDescending { get; set; } = string.Empty;
        public string IsIncluded { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string IndexId { get; set; } = string.Empty;
        public Index Index { get; set; } = new Index();
        public string TableColumnId { get; set; } = string.Empty;
        public TableColumn TableColumn { get; set; } = new TableColumn();
    }
}
