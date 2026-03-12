namespace MetaSchema
{
    public sealed class TableKeyField
    {
        public string Id { get; internal set; } = string.Empty;
        public string FieldName { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string FieldId { get; internal set; } = string.Empty;
        public Field Field { get; internal set; } = new Field();
        public string TableKeyId { get; internal set; } = string.Empty;
        public TableKey TableKey { get; internal set; } = new TableKey();
    }
}
