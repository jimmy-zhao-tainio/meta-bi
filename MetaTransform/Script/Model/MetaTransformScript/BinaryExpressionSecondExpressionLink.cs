#nullable enable

namespace MetaTransformScript
{
    public sealed class BinaryExpressionSecondExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public BinaryExpression BinaryExpression { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
