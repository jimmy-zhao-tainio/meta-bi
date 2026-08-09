#nullable enable

namespace MetaTransformScript
{
    public sealed class BooleanParenthesisExpression
    {
        public string Id { get; set; } = string.Empty;

        public BooleanExpression BooleanExpression { get; set; } = null!;

    }
}
