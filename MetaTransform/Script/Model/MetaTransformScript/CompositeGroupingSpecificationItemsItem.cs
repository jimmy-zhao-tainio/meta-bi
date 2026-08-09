#nullable enable

namespace MetaTransformScript
{
    public sealed class CompositeGroupingSpecificationItemsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public CompositeGroupingSpecification CompositeGroupingSpecification { get; set; } = null!;

        public GroupingSpecification GroupingSpecification { get; set; } = null!;

    }
}
