#nullable enable

namespace MetaTransformScript
{
    public sealed class SimpleCaseExpressionInputExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public SimpleCaseExpression SimpleCaseExpression { get; set; } = null!;

    }
}
