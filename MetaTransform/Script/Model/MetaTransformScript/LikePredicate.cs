#nullable enable

namespace MetaTransformScript
{
    public sealed class LikePredicate
    {
        public string Id { get; set; } = string.Empty;

        public string? NotDefined { get; set; }

        public string? OdbcEscape { get; set; }

        public BooleanExpression BooleanExpression { get; set; } = null!;

    }
}
