#nullable enable

namespace MetaTransformScript
{
    public sealed class SubqueryComparisonPredicateSubqueryLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarSubquery ScalarSubquery { get; set; } = null!;

        public SubqueryComparisonPredicate SubqueryComparisonPredicate { get; set; } = null!;

    }
}
