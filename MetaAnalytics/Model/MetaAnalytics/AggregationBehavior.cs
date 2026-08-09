#nullable enable

namespace MetaAnalytics
{
    public sealed class AggregationBehavior
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Function { get; set; } = string.Empty;

        public Measure Measure { get; set; } = null!;

    }
}
