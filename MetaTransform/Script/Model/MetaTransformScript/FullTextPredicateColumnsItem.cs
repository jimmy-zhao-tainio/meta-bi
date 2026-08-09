#nullable enable

namespace MetaTransformScript
{
    public sealed class FullTextPredicateColumnsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null!;

        public FullTextPredicate FullTextPredicate { get; set; } = null!;

    }
}
