#nullable enable

namespace MetaTransformScript
{
    public sealed class TableReferenceWithAliasAndColumnsColumnsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public Identifier Identifier { get; set; } = null!;

        public TableReferenceWithAliasAndColumns TableReferenceWithAliasAndColumns { get; set; } = null!;

    }
}
