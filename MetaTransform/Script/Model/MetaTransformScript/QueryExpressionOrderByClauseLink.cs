#nullable enable

namespace MetaTransformScript
{
    public sealed class QueryExpressionOrderByClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public OrderByClause OrderByClause { get; set; } = null!;

        public QueryExpression QueryExpression { get; set; } = null!;

    }
}
