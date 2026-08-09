#nullable enable

namespace MetaTransformScript
{
    public sealed class QueryParenthesisExpression
    {
        public string Id { get; set; } = string.Empty;

        public QueryExpression QueryExpression { get; set; } = null!;

    }
}
