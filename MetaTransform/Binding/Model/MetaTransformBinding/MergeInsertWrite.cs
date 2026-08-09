#nullable enable

namespace MetaTransformBinding
{
    public sealed class MergeInsertWrite
    {
        public string Id { get; set; } = string.Empty;

        public string MetaTransformScriptMergeInsertActionId { get; set; } = string.Empty;

        public Write Write { get; set; } = null!;

    }
}
