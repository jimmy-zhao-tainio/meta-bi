#nullable enable

namespace MetaTransformScript
{
    public sealed class UnpivotedTableReference
    {
        public string Id { get; set; } = string.Empty;

        public TableReferenceWithAlias TableReferenceWithAlias { get; set; } = null!;

    }
}
