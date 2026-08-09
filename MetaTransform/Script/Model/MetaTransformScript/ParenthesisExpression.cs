#nullable enable

namespace MetaTransformScript
{
    public sealed class ParenthesisExpression
    {
        public string Id { get; set; } = string.Empty;

        public PrimaryExpression PrimaryExpression { get; set; } = null!;

    }
}
