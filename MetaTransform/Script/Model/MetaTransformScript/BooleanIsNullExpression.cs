#nullable enable

namespace MetaTransformScript
{
    public sealed class BooleanIsNullExpression
    {
        public string Id { get; set; } = string.Empty;

        public string? IsNot { get; set; }

        public BooleanExpression BooleanExpression { get; set; } = null!;

    }
}
