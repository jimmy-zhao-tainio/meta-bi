#nullable enable

namespace MetaTransformScript
{
    public sealed class SelectScalarExpressionColumnNameLink
    {
        public string Id { get; set; } = string.Empty;

        public IdentifierOrValueExpression IdentifierOrValueExpression { get; set; } = null!;

        public SelectScalarExpression SelectScalarExpression { get; set; } = null!;

    }
}
