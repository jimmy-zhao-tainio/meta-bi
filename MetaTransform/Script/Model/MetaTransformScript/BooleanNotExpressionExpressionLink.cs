#nullable enable

namespace MetaTransformScript
{
    public sealed class BooleanNotExpressionExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public BooleanExpression BooleanExpression { get; set; } = null!;

        public BooleanNotExpression BooleanNotExpression { get; set; } = null!;

    }
}
