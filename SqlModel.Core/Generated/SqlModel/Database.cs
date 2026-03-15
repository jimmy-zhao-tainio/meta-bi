namespace SqlModel
{
    public sealed class Database
    {
        public string Id { get; internal set; } = string.Empty;
        public string Collation { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Platform { get; internal set; } = string.Empty;
    }
}
