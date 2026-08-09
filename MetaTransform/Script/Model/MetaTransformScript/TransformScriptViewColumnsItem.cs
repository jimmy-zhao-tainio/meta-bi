#nullable enable

namespace MetaTransformScript
{
    public sealed class TransformScriptViewColumnsItem
    {
        public string Id { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public Identifier Identifier { get; set; } = null!;

        public TransformScript TransformScript { get; set; } = null!;

    }
}
