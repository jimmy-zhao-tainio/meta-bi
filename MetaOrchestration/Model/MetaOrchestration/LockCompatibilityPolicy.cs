#nullable enable

namespace MetaOrchestration
{
    public sealed class LockCompatibilityPolicy
    {
        public string Id { get; set; } = string.Empty;

        public string LeftEffect { get; set; } = string.Empty;

        public string LockBehavior { get; set; } = string.Empty;

        public string PolicyKind { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public string RightEffect { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DataObject DataObject { get; set; } = null!;

        public DependencyIssue? DependencyIssue { get; set; }

        public OrchestrationPlan OrchestrationPlan { get; set; } = null!;

    }
}
