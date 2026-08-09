#nullable enable

namespace MetaTransformScript
{
    public sealed class UnaryExpression
    {
        public string Id { get; set; } = string.Empty;

        public string? UnaryExpressionType { get; set; }

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
