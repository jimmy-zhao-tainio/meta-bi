#nullable enable

namespace MetaTransformScript
{
    public sealed class UnaryExpressionExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public UnaryExpression UnaryExpression { get; set; } = null!;

    }
}
