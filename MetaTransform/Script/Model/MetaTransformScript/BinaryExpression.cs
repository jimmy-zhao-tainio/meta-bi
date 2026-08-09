#nullable enable

namespace MetaTransformScript
{
    public sealed class BinaryExpression
    {
        public string Id { get; set; } = string.Empty;

        public string? BinaryExpressionType { get; set; }

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
