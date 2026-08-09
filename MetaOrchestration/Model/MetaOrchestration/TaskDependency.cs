#nullable enable

namespace MetaOrchestration
{
    public sealed class TaskDependency
    {
        public string Id { get; set; } = string.Empty;

        public string DependencyCondition { get; set; } = string.Empty;

        public string DependencyKind { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public DataObject? DataObject { get; set; }

        public OrchestrationPlan OrchestrationPlan { get; set; } = null!;

        public TaskAccessProfile Predecessor { get; set; } = null!;

        public TaskAccessProfile Successor { get; set; } = null!;

    }
}
