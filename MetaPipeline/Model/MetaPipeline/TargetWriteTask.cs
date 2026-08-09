#nullable enable

namespace MetaPipeline
{
    public sealed class TargetWriteTask
    {
        public string Id { get; set; } = string.Empty;

        public PipelineTask PipelineTask { get; set; } = null!;

        public ConnectionReference TargetConnectionReference { get; set; } = null!;

    }
}
