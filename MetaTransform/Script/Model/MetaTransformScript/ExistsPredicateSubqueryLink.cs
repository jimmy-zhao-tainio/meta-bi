#nullable enable

namespace MetaTransformScript
{
    public sealed class ExistsPredicateSubqueryLink
    {
        public string Id { get; set; } = string.Empty;

        public ExistsPredicate ExistsPredicate { get; set; } = null!;

        public ScalarSubquery ScalarSubquery { get; set; } = null!;

    }
}
