#nullable enable

namespace MetaTransformScript
{
    public sealed class MultiPartIdentifierCallTarget
    {
        public string Id { get; set; } = string.Empty;

        public CallTarget CallTarget { get; set; } = null!;

    }
}
