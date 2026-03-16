namespace MetaSchema
{
    public sealed class Field
    {
        public string Id { get; set; } = string.Empty;
        public string IsNullable { get; set; } = string.Empty;
        public string MetaDataTypeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string TableId { get; set; } = string.Empty;
        public Table Table { get; set; } = new Table();
    }
}
