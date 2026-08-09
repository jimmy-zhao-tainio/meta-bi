#nullable enable

namespace MetaTransformScript
{
    public sealed class WhenClauseThenExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public WhenClause WhenClause { get; set; } = null!;

    }
}
