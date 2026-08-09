#nullable enable

namespace MetaOrchestration
{
    public sealed class RetryPolicyFailureClass
    {
        public string Id { get; set; } = string.Empty;

        public string FailureClass { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public string RetryBehavior { get; set; } = string.Empty;

        public RetryPolicy RetryPolicy { get; set; } = null!;

    }
}
