#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeDeleteAction
    {
        public string Id { get; set; } = string.Empty;

        public MergeAction MergeAction { get; set; } = null!;

    }
}
