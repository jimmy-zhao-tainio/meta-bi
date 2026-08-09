#nullable enable

namespace MetaTransformScript
{
    public sealed class SelectStatementQueryExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public QueryExpression QueryExpression { get; set; } = null!;

        public SelectStatement SelectStatement { get; set; } = null!;

    }
}
