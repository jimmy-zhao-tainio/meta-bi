#nullable enable

namespace MetaOrchestration
{
    public sealed class RetryPolicy
    {
        public string Id { get; set; } = string.Empty;

        public string BackoffMultiplier { get; set; } = string.Empty;

        public string InitialDelayMilliseconds { get; set; } = string.Empty;

        public string MaxAttempts { get; set; } = string.Empty;

        public string MaxDelayMilliseconds { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string PolicyKind { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public string RetryReadOnlyTasksByDefault { get; set; } = string.Empty;

        public string RetryWriteTasksByDefault { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public OrchestrationPlan OrchestrationPlan { get; set; } = null!;

    }
}
