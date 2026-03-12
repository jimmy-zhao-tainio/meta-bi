namespace MetaSchema
{
    public sealed class FieldDataTypeDetail
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Value { get; internal set; } = string.Empty;
        public string FieldId { get; internal set; } = string.Empty;
        public Field Field { get; internal set; } = new Field();
    }
}
