namespace SqlModel
{
    public sealed class Table
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SchemaId { get; set; } = string.Empty;
        public Schema Schema { get; set; } = new Schema();
    }
}
