#nullable enable

namespace MetaSchema
{
    public sealed class SchemaObject
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Schema Schema { get; set; } = null!;

    }
}
