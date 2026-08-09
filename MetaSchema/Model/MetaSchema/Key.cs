#nullable enable

namespace MetaSchema
{
    public sealed class Key
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Table Table { get; set; } = null!;

    }
}
