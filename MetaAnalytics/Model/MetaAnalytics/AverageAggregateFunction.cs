#nullable enable

namespace MetaAnalytics
{
    public sealed class AverageAggregateFunction
    {
        public string Id { get; set; } = string.Empty;

        public AggregateFunction AggregateFunction { get; set; } = null!;
    }
}
