#nullable enable

namespace MetaTransformScript
{
    public sealed class TransformScriptObjectIdentifierLink
    {
        public string Id { get; set; } = string.Empty;

        public Identifier Identifier { get; set; } = null!;

        public TransformScript TransformScript { get; set; } = null!;

    }
}
