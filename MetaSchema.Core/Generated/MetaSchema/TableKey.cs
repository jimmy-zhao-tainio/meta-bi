namespace MetaSchema
{
    public sealed class TableKey
    {
        public string Id { get; internal set; } = string.Empty;
        public string KeyType { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string TableId { get; internal set; } = string.Empty;
        public Table Table { get; internal set; } = new Table();
    }
}
