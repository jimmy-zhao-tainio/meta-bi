#nullable enable

namespace MetaTransformScript
{
    public sealed class InPredicateValuesItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public InPredicate InPredicate { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
