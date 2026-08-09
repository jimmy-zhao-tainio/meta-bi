#nullable enable

namespace MetaPipeline
{
    public sealed class RowStreamProducer
    {
        public string Id { get; set; } = string.Empty;

        public PipelineTask PipelineTask { get; set; } = null!;

        public RowStream RowStream { get; set; } = null!;

    }
}
