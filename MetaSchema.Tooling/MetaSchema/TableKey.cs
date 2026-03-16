namespace MetaSchema
{
    public sealed class TableKey
    {
        public string Id { get; set; } = string.Empty;
        public string KeyType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string TableId { get; set; } = string.Empty;
        public Table Table { get; set; } = new Table();
    }
}
