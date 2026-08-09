#nullable enable

namespace MetaTransformScript
{
    public sealed class SimpleWhenClauseWhenExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public SimpleWhenClause SimpleWhenClause { get; set; } = null!;

    }
}
