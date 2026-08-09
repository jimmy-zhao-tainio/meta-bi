#nullable enable

namespace MetaTransformBinding
{
    public sealed class MergeDelete
    {
        public string Id { get; set; } = string.Empty;

        public string MetaTransformScriptMergeDeleteActionId { get; set; } = string.Empty;

        public ValidationTargetRowsetLink ValidationTargetRowsetLink { get; set; } = null!;

    }
}
