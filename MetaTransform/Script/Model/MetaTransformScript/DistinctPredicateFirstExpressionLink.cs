#nullable enable

namespace MetaTransformScript
{
    public sealed class DistinctPredicateFirstExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public DistinctPredicate DistinctPredicate { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
