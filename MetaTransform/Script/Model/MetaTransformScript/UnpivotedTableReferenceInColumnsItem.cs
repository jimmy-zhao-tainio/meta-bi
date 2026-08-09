#nullable enable

namespace MetaTransformScript
{
    public sealed class UnpivotedTableReferenceInColumnsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null!;

        public UnpivotedTableReference UnpivotedTableReference { get; set; } = null!;

    }
}
