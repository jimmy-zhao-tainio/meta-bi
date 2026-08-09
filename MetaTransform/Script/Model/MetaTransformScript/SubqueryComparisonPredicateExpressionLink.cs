#nullable enable

namespace MetaTransformScript
{
    public sealed class SubqueryComparisonPredicateExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public SubqueryComparisonPredicate SubqueryComparisonPredicate { get; set; } = null!;

    }
}
