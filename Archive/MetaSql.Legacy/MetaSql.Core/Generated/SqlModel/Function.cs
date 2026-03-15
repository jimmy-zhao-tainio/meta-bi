namespace SqlModel
{
    public sealed class Function
    {
        public string Id { get; internal set; } = string.Empty;
        public string DefinitionSql { get; internal set; } = string.Empty;
        public string FunctionKind { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string ReturnLength { get; internal set; } = string.Empty;
        public string ReturnPrecision { get; internal set; } = string.Empty;
        public string ReturnScale { get; internal set; } = string.Empty;
        public string ReturnTypeName { get; internal set; } = string.Empty;
        public string SchemaId { get; internal set; } = string.Empty;
        public Schema Schema { get; internal set; } = new Schema();
    }
}
