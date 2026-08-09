#nullable enable

namespace MetaTransformScript
{
    public sealed class NullLiteral
    {
        public string Id { get; set; } = string.Empty;

        public string? LiteralType { get; set; }

        public Literal Literal { get; set; } = null!;

    }
}
