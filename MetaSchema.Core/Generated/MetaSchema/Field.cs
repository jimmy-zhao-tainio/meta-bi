namespace MetaSchema
{
    public sealed class Field
    {
        public string Id { get; internal set; } = string.Empty;
        public string DataTypeId { get; internal set; } = string.Empty;
        public string IsNullable { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string TableId { get; internal set; } = string.Empty;
        public Table Table { get; internal set; } = new Table();
    }
}
