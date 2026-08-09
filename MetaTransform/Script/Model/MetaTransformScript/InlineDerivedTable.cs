#nullable enable

namespace MetaTransformScript
{
    public sealed class InlineDerivedTable
    {
        public string Id { get; set; } = string.Empty;

        public TableReferenceWithAliasAndColumns TableReferenceWithAliasAndColumns { get; set; } = null!;

    }
}
