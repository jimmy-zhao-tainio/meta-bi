#nullable enable

namespace MetaTransformScript
{
    public sealed class BooleanComparisonExpressionSecondExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public BooleanComparisonExpression BooleanComparisonExpression { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
