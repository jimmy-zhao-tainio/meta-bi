#nullable enable

namespace MetaTransformScript
{
    public sealed class BooleanComparisonExpression
    {
        public string Id { get; set; } = string.Empty;

        public string? ComparisonType { get; set; }

        public BooleanExpression BooleanExpression { get; set; } = null!;

    }
}
