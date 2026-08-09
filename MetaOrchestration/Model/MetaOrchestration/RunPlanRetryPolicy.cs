#nullable enable

namespace MetaOrchestration
{
    public sealed class RunPlanRetryPolicy
    {
        public string Id { get; set; } = string.Empty;

        public string PolicyRole { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public RetryPolicy RetryPolicy { get; set; } = null!;

        public RunPlan RunPlan { get; set; } = null!;

    }
}
