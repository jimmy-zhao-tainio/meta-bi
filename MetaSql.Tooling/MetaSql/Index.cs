namespace MetaSql
{
    public sealed class Index
    {
        public string Id { get; set; } = string.Empty;
        public string FilterSql { get; set; } = string.Empty;
        public string IsClustered { get; set; } = string.Empty;
        public string IsUnique { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string TableId { get; set; } = string.Empty;
        public Table Table { get; set; } = new Table();
    }
}
