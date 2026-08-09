#nullable enable

namespace MetaTransformScript
{
    public sealed class MultiPartIdentifierIdentifiersItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public Identifier Identifier { get; set; } = null!;

        public MultiPartIdentifier MultiPartIdentifier { get; set; } = null!;

    }
}
