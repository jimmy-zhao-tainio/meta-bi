#nullable enable

namespace MetaTransformScript
{
    public sealed class TopRowFilterExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public TopRowFilter TopRowFilter { get; set; } = null!;

    }
}
