#nullable enable

namespace MetaTransformScript
{
    public sealed class RollupGroupingSpecificationArgumentsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public GroupingSpecification GroupingSpecification { get; set; } = null!;

        public RollupGroupingSpecification RollupGroupingSpecification { get; set; } = null!;

    }
}
