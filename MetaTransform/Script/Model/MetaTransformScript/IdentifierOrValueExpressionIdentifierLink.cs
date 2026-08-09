#nullable enable

namespace MetaTransformScript
{
    public sealed class IdentifierOrValueExpressionIdentifierLink
    {
        public string Id { get; set; } = string.Empty;

        public Identifier Identifier { get; set; } = null!;

        public IdentifierOrValueExpression IdentifierOrValueExpression { get; set; } = null!;

    }
}
