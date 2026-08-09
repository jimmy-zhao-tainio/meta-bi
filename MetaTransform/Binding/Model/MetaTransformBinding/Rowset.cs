#nullable enable

namespace MetaTransformBinding
{
    public sealed class Rowset
    {
        public string Id { get; set; } = string.Empty;

        public string DerivationKind { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? SqlIdentifier { get; set; }

        public TransformBinding TransformBinding { get; set; } = null!;

    }
}
