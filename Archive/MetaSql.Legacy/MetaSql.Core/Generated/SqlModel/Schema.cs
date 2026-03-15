namespace SqlModel
{
    public sealed class Schema
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string DatabaseId { get; internal set; } = string.Empty;
        public Database Database { get; internal set; } = new Database();
    }
}
