namespace SqlModel
{
    public sealed class PrimaryKeyColumn
    {
        public string Id { get; internal set; } = string.Empty;
        public string IsDescending { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string PrimaryKeyId { get; internal set; } = string.Empty;
        public PrimaryKey PrimaryKey { get; internal set; } = new PrimaryKey();
        public string TableColumnId { get; internal set; } = string.Empty;
        public TableColumn TableColumn { get; internal set; } = new TableColumn();
    }
}
