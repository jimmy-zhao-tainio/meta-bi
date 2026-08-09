#nullable enable

namespace MetaTransformScript
{
    public sealed class QueryParenthesisExpressionQueryExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public QueryExpression QueryExpression { get; set; } = null!;

        public QueryParenthesisExpression QueryParenthesisExpression { get; set; } = null!;

    }
}
