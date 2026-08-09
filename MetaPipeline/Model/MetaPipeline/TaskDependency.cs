#nullable enable

namespace MetaPipeline
{
    public sealed class TaskDependency
    {
        public string Id { get; set; } = string.Empty;

        public Pipeline Pipeline { get; set; } = null!;

        public PipelineTask Predecessor { get; set; } = null!;

        public PipelineTask Successor { get; set; } = null!;

    }
}
