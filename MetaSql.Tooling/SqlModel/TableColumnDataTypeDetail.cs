namespace SqlModel
{
    public sealed class TableColumnDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string TableColumnId { get; set; } = string.Empty;
        public TableColumn TableColumn { get; set; } = new TableColumn();
    }
}
