namespace MetaSchema
{
    public sealed class FieldDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string FieldId { get; set; } = string.Empty;
        public Field Field { get; set; } = new Field();
    }
}
