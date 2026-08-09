#nullable enable

namespace MetaTransformScript
{
    public sealed class ExpressionGroupingSpecificationExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public ExpressionGroupingSpecification ExpressionGroupingSpecification { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
