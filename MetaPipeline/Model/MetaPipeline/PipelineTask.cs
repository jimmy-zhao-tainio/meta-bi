#nullable enable

namespace MetaPipeline
{
    public sealed class PipelineTask
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Pipeline Pipeline { get; set; } = null!;

    }
}
