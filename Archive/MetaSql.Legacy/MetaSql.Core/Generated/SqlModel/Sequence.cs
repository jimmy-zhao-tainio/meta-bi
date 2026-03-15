namespace SqlModel
{
    public sealed class Sequence
    {
        public string Id { get; internal set; } = string.Empty;
        public string IncrementValue { get; internal set; } = string.Empty;
        public string IsCycling { get; internal set; } = string.Empty;
        public string MaximumValue { get; internal set; } = string.Empty;
        public string MinimumValue { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string StartValue { get; internal set; } = string.Empty;
        public string TypeName { get; internal set; } = string.Empty;
        public string SchemaId { get; internal set; } = string.Empty;
        public Schema Schema { get; internal set; } = new Schema();
    }
}
