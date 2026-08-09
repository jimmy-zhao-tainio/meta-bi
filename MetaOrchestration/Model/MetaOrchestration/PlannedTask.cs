#nullable enable

namespace MetaOrchestration
{
    public sealed class PlannedTask
    {
        public string Id { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public PipelineReference PipelineReference { get; set; } = null!;

        public RunPlan RunPlan { get; set; } = null!;

        public TaskAccessProfile TaskAccessProfile { get; set; } = null!;

    }
}
