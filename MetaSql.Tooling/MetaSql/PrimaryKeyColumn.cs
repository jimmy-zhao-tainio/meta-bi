namespace MetaSql
{
    public sealed class PrimaryKeyColumn
    {
        public string Id { get; set; } = string.Empty;
        public string IsDescending { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string PrimaryKeyId { get; set; } = string.Empty;
        public PrimaryKey PrimaryKey { get; set; } = new PrimaryKey();
        public string TableColumnId { get; set; } = string.Empty;
        public TableColumn TableColumn { get; set; } = new TableColumn();
    }
}
