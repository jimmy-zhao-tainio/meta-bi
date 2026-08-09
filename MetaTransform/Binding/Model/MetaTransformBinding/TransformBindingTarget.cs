#nullable enable

namespace MetaTransformBinding
{
    public sealed class TransformBindingTarget
    {
        public string Id { get; set; } = string.Empty;

        public string SqlIdentifier { get; set; } = string.Empty;

        public TransformBinding TransformBinding { get; set; } = null!;

    }
}
