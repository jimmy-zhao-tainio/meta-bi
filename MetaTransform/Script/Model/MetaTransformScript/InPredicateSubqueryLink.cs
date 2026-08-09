#nullable enable

namespace MetaTransformScript
{
    public sealed class InPredicateSubqueryLink
    {
        public string Id { get; set; } = string.Empty;

        public InPredicate InPredicate { get; set; } = null!;

        public ScalarSubquery ScalarSubquery { get; set; } = null!;

    }
}
