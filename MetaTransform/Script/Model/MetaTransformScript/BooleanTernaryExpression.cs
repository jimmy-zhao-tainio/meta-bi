#nullable enable

namespace MetaTransformScript
{
    public sealed class BooleanTernaryExpression
    {
        public string Id { get; set; } = string.Empty;

        public string? TernaryExpressionType { get; set; }

        public BooleanExpression BooleanExpression { get; set; } = null!;

    }
}
