#nullable enable

namespace MetaAnalytics
{
    public sealed class MaximumAggregateFunction
    {
        public string Id { get; set; } = string.Empty;

        public AggregateFunction AggregateFunction { get; set; } = null!;
    }
}
