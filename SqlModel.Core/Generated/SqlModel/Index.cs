namespace SqlModel
{
    public sealed class Index
    {
        public string Id { get; internal set; } = string.Empty;
        public string FilterSql { get; internal set; } = string.Empty;
        public string IsClustered { get; internal set; } = string.Empty;
        public string IsUnique { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string TableId { get; internal set; } = string.Empty;
        public Table Table { get; internal set; } = new Table();
    }
}
