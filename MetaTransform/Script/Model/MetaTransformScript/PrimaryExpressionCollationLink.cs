#nullable enable

namespace MetaTransformScript
{
    public sealed class PrimaryExpressionCollationLink
    {
        public string Id { get; set; } = string.Empty;

        public Identifier Identifier { get; set; } = null!;

        public PrimaryExpression PrimaryExpression { get; set; } = null!;

    }
}
