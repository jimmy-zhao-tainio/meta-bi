#nullable enable

namespace MetaTransformScript
{
    public sealed class SimpleCaseExpressionWhenClausesItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public SimpleCaseExpression SimpleCaseExpression { get; set; } = null!;

        public SimpleWhenClause SimpleWhenClause { get; set; } = null!;

    }
}
