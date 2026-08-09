#nullable enable

namespace MetaTransformScript
{
    public sealed class SearchedCaseExpressionWhenClausesItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public SearchedCaseExpression SearchedCaseExpression { get; set; } = null!;

        public SearchedWhenClause SearchedWhenClause { get; set; } = null!;

    }
}
