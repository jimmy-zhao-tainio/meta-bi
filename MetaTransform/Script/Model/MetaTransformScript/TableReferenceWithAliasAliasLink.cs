#nullable enable

namespace MetaTransformScript
{
    public sealed class TableReferenceWithAliasAliasLink
    {
        public string Id { get; set; } = string.Empty;

        public Identifier Identifier { get; set; } = null!;

        public TableReferenceWithAlias TableReferenceWithAlias { get; set; } = null!;

    }
}
