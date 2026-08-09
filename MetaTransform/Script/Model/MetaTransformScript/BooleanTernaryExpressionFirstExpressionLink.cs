#nullable enable

namespace MetaTransformScript
{
    public sealed class BooleanTernaryExpressionFirstExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public BooleanTernaryExpression BooleanTernaryExpression { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
