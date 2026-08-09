#nullable enable

namespace MetaTransformScript
{
    public sealed class FullTextPredicateValueLink
    {
        public string Id { get; set; } = string.Empty;

        public FullTextPredicate FullTextPredicate { get; set; } = null!;

        public ValueExpression ValueExpression { get; set; } = null!;

    }
}
