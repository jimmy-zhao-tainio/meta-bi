namespace SqlModel
{
    public sealed class Schema
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DatabaseId { get; set; } = string.Empty;
        public Database Database { get; set; } = new Database();
    }
}
