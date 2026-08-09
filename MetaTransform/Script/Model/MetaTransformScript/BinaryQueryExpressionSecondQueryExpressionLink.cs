#nullable enable

namespace MetaTransformScript
{
    public sealed class BinaryQueryExpressionSecondQueryExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public BinaryQueryExpression BinaryQueryExpression { get; set; } = null!;

        public QueryExpression QueryExpression { get; set; } = null!;

    }
}
