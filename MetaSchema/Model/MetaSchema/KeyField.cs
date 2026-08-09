#nullable enable

namespace MetaSchema
{
    public sealed class KeyField
    {
        public string Id { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public Field Field { get; set; } = null!;

        public Key Key { get; set; } = null!;

    }
}
