#nullable enable

namespace MetaTransformScript
{
    public sealed class BooleanBinaryExpressionFirstExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public BooleanBinaryExpression BooleanBinaryExpression { get; set; } = null!;

        public BooleanExpression BooleanExpression { get; set; } = null!;

    }
}
