#nullable enable

namespace MetaTransformScript
{
    public sealed class OrderByClauseOrderByElementsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public ExpressionWithSortOrder ExpressionWithSortOrder { get; set; } = null!;

        public OrderByClause OrderByClause { get; set; } = null!;

    }
}
