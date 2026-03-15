namespace SqlModel
{
    public sealed class UserDefinedType
    {
        public string Id { get; internal set; } = string.Empty;
        public string BaseTypeName { get; internal set; } = string.Empty;
        public string Length { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Precision { get; internal set; } = string.Empty;
        public string Scale { get; internal set; } = string.Empty;
        public string SchemaId { get; internal set; } = string.Empty;
        public Schema Schema { get; internal set; } = new Schema();
    }
}
