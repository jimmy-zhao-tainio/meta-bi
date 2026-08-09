#nullable enable

namespace MetaOrchestration
{
    public sealed class TaskOrderingResolution
    {
        public string Id { get; set; } = string.Empty;

        public string DependencyCondition { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public string ResolutionKind { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DataObject? DataObject { get; set; }

        public DependencyIssue? DependencyIssue { get; set; }

        public OrchestrationPlan OrchestrationPlan { get; set; } = null!;

        public TaskAccessProfile Predecessor { get; set; } = null!;

        public TaskAccessProfile Successor { get; set; } = null!;

    }
}
