#nullable enable

namespace MetaTransformScript
{
    public sealed class SelectScalarExpressionExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public SelectScalarExpression SelectScalarExpression { get; set; } = null!;

    }
}
