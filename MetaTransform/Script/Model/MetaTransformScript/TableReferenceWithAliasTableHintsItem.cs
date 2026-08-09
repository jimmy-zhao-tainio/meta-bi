#nullable enable

namespace MetaTransformScript
{
    public sealed class TableReferenceWithAliasTableHintsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public SqlHint SqlHint { get; set; } = null!;

        public TableReferenceWithAlias TableReferenceWithAlias { get; set; } = null!;

    }
}
