#nullable enable

namespace MetaTransformBinding
{
    public sealed class MergeUpdateWrite
    {
        public string Id { get; set; } = string.Empty;

        public string MetaTransformScriptMergeUpdateActionId { get; set; } = string.Empty;

        public Write Write { get; set; } = null!;

    }
}
