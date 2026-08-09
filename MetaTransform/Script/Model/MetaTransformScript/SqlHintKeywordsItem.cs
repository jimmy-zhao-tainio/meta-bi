#nullable enable

namespace MetaTransformScript
{
    public sealed class SqlHintKeywordsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public Identifier Identifier { get; set; } = null!;

        public SqlHint SqlHint { get; set; } = null!;

    }
}
