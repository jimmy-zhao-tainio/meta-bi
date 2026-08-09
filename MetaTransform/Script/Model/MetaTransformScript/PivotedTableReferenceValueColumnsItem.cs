#nullable enable

namespace MetaTransformScript
{
    public sealed class PivotedTableReferenceValueColumnsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null!;

        public PivotedTableReference PivotedTableReference { get; set; } = null!;

    }
}
