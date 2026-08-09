#nullable enable

namespace MetaOrchestration
{
    public sealed class PipelineDependency
    {
        public string Id { get; set; } = string.Empty;

        public string DependencyKind { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public OrchestrationPlan OrchestrationPlan { get; set; } = null!;

        public PipelineReference Predecessor { get; set; } = null!;

        public PipelineReference Successor { get; set; } = null!;

    }
}
