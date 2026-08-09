#nullable enable

namespace MetaTransformScript
{
    public sealed class CoalesceExpressionExpressionsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public CoalesceExpression CoalesceExpression { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
