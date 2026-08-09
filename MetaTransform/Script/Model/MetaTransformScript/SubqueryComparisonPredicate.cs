#nullable enable

namespace MetaTransformScript
{
    public sealed class SubqueryComparisonPredicate
    {
        public string Id { get; set; } = string.Empty;

        public string? ComparisonType { get; set; }

        public string? SubqueryComparisonPredicateType { get; set; }

        public BooleanExpression BooleanExpression { get; set; } = null!;

    }
}
