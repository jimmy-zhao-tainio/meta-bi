#nullable enable

namespace MetaTransformScript
{
    public sealed class Literal
    {
        public string Id { get; set; } = string.Empty;

        public string? LiteralType { get; set; }

        public string? Value { get; set; }

        public ValueExpression ValueExpression { get; set; } = null!;

    }
}
