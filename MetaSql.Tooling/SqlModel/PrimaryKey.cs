namespace SqlModel
{
    public sealed class PrimaryKey
    {
        public string Id { get; set; } = string.Empty;
        public string IsClustered { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string TableId { get; set; } = string.Empty;
        public Table Table { get; set; } = new Table();
    }
}
