#nullable enable

namespace MetaSchema
{
    public sealed class UniqueKey
    {
        public string Id { get; set; } = string.Empty;

        public Key Key { get; set; } = null!;

    }
}
