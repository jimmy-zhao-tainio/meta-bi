#nullable enable

namespace MetaTransformScript
{
    public sealed class InPredicate
    {
        public string Id { get; set; } = string.Empty;

        public string? NotDefined { get; set; }

        public BooleanExpression BooleanExpression { get; set; } = null!;

    }
}
