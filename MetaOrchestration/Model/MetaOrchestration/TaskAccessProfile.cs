#nullable enable

namespace MetaOrchestration
{
    public sealed class TaskAccessProfile
    {
        public string Id { get; set; } = string.Empty;

        public string? BindingWorkspacePath { get; set; }

        public string MetaPipelinePipelineTaskId { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public string StatementKind { get; set; } = string.Empty;

        public string TaskKind { get; set; } = string.Empty;

        public string TaskName { get; set; } = string.Empty;

        public string? TransformBindingId { get; set; }

        public string? TransformScriptId { get; set; }

        public string? TransformScriptName { get; set; }

        public string? TransformWorkspacePath { get; set; }

        public PipelineReference PipelineReference { get; set; } = null!;

    }
}
