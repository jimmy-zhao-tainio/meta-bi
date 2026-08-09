#nullable enable

namespace MetaPipeline
{
    public sealed class RowStreamConsumer
    {
        public string Id { get; set; } = string.Empty;

        public PipelineTask PipelineTask { get; set; } = null!;

        public RowStream RowStream { get; set; } = null!;

    }
}
