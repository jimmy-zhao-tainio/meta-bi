#nullable enable

namespace MetaTransformScript
{
    public sealed class FullTextTableReferenceColumnsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null!;

        public FullTextTableReference FullTextTableReference { get; set; } = null!;

    }
}
