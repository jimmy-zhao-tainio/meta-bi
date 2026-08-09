#nullable enable

namespace MetaSchema
{
    public sealed class PrimaryKey
    {
        public string Id { get; set; } = string.Empty;

        public Key Key { get; set; } = null!;

    }
}
