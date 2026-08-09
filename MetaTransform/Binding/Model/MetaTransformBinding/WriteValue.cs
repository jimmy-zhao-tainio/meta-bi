#nullable enable

namespace MetaTransformBinding
{
    public sealed class WriteValue
    {
        public string Id { get; set; } = string.Empty;

        public ValidationTargetColumnLink ValidationTargetColumnLink { get; set; } = null!;

        public Write Write { get; set; } = null!;

    }
}
