#nullable enable

namespace MetaOrchestration
{
    public sealed class RunPlan
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public string RunPlanStatus { get; set; } = string.Empty;

        public OrchestrationPlan OrchestrationPlan { get; set; } = null!;

    }
}
