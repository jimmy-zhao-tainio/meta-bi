#nullable enable

namespace MetaSchema
{
    public sealed class View
    {
        public string Id { get; set; } = string.Empty;

        public SchemaObject SchemaObject { get; set; } = null!;

    }
}
