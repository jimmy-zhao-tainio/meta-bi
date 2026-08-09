#nullable enable

namespace MetaTransformScript
{
    public sealed class CommonTableExpressionQueryExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public CommonTableExpression CommonTableExpression { get; set; } = null!;

        public QueryExpression QueryExpression { get; set; } = null!;

    }
}
