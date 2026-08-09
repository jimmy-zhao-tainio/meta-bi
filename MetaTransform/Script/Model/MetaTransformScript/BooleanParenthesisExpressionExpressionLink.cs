#nullable enable

namespace MetaTransformScript
{
    public sealed class BooleanParenthesisExpressionExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public BooleanExpression BooleanExpression { get; set; } = null!;

        public BooleanParenthesisExpression BooleanParenthesisExpression { get; set; } = null!;

    }
}
