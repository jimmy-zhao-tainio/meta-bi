#nullable enable

namespace MetaTransformScript
{
    public sealed class QueryDerivedTableQueryExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public QueryDerivedTable QueryDerivedTable { get; set; } = null!;

        public QueryExpression QueryExpression { get; set; } = null!;

    }
}
