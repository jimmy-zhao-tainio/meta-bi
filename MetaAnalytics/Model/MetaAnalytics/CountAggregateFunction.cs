#nullable enable

namespace MetaAnalytics
{
    public sealed class CountAggregateFunction
    {
        public string Id { get; set; } = string.Empty;

        public AggregateFunction AggregateFunction { get; set; } = null!;
    }
}
