#nullable enable

namespace MetaOrchestration
{
    public sealed class OrchestrationPlan
    {
        public string Id { get; set; } = string.Empty;

        public string DagStatus { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string DeterminismStatus { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string SynchronizationStatus { get; set; } = string.Empty;

    }
}
