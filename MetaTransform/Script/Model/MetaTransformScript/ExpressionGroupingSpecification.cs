#nullable enable

namespace MetaTransformScript
{
    public sealed class ExpressionGroupingSpecification
    {
        public string Id { get; set; } = string.Empty;

        public string? DistributedAggregation { get; set; }

        public GroupingSpecification GroupingSpecification { get; set; } = null!;

    }
}
