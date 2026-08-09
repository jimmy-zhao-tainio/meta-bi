#nullable enable

namespace MetaTransformScript
{
    public sealed class UnpivotedTableReferencePivotColumnLink
    {
        public string Id { get; set; } = string.Empty;

        public Identifier Identifier { get; set; } = null!;

        public UnpivotedTableReference UnpivotedTableReference { get; set; } = null!;

    }
}
