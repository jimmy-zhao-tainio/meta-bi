#nullable enable

namespace MetaTransformScript
{
    public sealed class BooleanBinaryExpression
    {
        public string Id { get; set; } = string.Empty;

        public string? BinaryExpressionType { get; set; }

        public BooleanExpression BooleanExpression { get; set; } = null!;

    }
}
