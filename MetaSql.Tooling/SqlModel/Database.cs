namespace SqlModel
{
    public sealed class Database
    {
        public string Id { get; set; } = string.Empty;
        public string Collation { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
    }
}
