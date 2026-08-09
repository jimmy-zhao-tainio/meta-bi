#nullable enable

namespace MetaTransformScript
{
    public sealed class ParenthesisExpressionExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public ParenthesisExpression ParenthesisExpression { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
