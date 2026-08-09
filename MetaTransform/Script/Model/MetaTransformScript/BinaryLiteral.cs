#nullable enable

namespace MetaTransformScript
{
    public sealed class BinaryLiteral
    {
        public string Id { get; set; } = string.Empty;

        public string? IsLargeObject { get; set; }

        public string? LiteralType { get; set; }

        public Literal Literal { get; set; } = null!;

    }
}
