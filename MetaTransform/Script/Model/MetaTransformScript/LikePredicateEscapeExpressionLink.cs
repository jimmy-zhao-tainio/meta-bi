#nullable enable

namespace MetaTransformScript
{
    public sealed class LikePredicateEscapeExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public LikePredicate LikePredicate { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
