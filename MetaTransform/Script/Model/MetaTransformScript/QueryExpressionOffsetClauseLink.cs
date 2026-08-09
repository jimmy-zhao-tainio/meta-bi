#nullable enable

namespace MetaTransformScript
{
    public sealed class QueryExpressionOffsetClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public OffsetClause OffsetClause { get; set; } = null!;

        public QueryExpression QueryExpression { get; set; } = null!;

    }
}
