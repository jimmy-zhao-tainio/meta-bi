#nullable enable

namespace MetaTransformScript
{
    public sealed class PivotedTableReferencePivotColumnLink
    {
        public string Id { get; set; } = string.Empty;

        public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null!;

        public PivotedTableReference PivotedTableReference { get; set; } = null!;

    }
}
