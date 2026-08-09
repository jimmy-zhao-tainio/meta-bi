#nullable enable

namespace MetaTransformScript
{
    public sealed class ExpressionWithSortOrderExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public ExpressionWithSortOrder ExpressionWithSortOrder { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
