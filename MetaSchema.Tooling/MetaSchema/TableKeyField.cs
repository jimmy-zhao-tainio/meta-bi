namespace MetaSchema
{
    public sealed class TableKeyField
    {
        public string Id { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string FieldId { get; set; } = string.Empty;
        public Field Field { get; set; } = new Field();
        public string TableKeyId { get; set; } = string.Empty;
        public TableKey TableKey { get; set; } = new TableKey();
    }
}
