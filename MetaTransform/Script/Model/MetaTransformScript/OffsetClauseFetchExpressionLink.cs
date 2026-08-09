#nullable enable

namespace MetaTransformScript
{
    public sealed class OffsetClauseFetchExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public OffsetClause OffsetClause { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
