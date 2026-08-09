#nullable enable

namespace MetaTransformScript
{
    public sealed class SearchedWhenClauseWhenExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public BooleanExpression BooleanExpression { get; set; } = null!;

        public SearchedWhenClause SearchedWhenClause { get; set; } = null!;

    }
}
