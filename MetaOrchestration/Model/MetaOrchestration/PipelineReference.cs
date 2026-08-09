#nullable enable

namespace MetaOrchestration
{
    public sealed class PipelineReference
    {
        public string Id { get; set; } = string.Empty;

        public string MetaPipelinePipelineId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? PipelineWorkspacePath { get; set; }

        public OrchestrationPlan OrchestrationPlan { get; set; } = null!;

    }
}
