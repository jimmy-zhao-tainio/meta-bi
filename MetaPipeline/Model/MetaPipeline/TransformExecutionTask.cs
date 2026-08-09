#nullable enable

namespace MetaPipeline
{
    public sealed class TransformExecutionTask
    {
        public string Id { get; set; } = string.Empty;

        public string BindingWorkspacePath { get; set; } = string.Empty;

        public string? TimeoutSeconds { get; set; }

        public string TransformBindingId { get; set; } = string.Empty;

        public string TransformScriptId { get; set; } = string.Empty;

        public string TransformWorkspacePath { get; set; } = string.Empty;

        public ConnectionReference ExecutionConnectionReference { get; set; } = null!;

        public PipelineTask PipelineTask { get; set; } = null!;

    }
}
