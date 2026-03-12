namespace MetaSchema
{
    public sealed class Table
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string ObjectType { get; internal set; } = string.Empty;
        public string SchemaId { get; internal set; } = string.Empty;
        public Schema Schema { get; internal set; } = new Schema();
    }
}
