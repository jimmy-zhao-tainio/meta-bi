#nullable enable

namespace MetaTransformScript
{
    public sealed class ScalarSubqueryQueryExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public QueryExpression QueryExpression { get; set; } = null!;

        public ScalarSubquery ScalarSubquery { get; set; } = null!;

    }
}
