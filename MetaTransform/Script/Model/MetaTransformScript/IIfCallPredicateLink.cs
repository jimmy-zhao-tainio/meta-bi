#nullable enable

namespace MetaTransformScript
{
    public sealed class IIfCallPredicateLink
    {
        public string Id { get; set; } = string.Empty;

        public BooleanExpression BooleanExpression { get; set; } = null!;

        public IIfCall IIfCall { get; set; } = null!;

    }
}
