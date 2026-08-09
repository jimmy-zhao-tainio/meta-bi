#nullable enable

namespace MetaTransformScript
{
    public sealed class GroupingSetsGroupingSpecificationSetsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public GroupingSetsGroupingSpecification GroupingSetsGroupingSpecification { get; set; } = null!;

        public GroupingSpecification GroupingSpecification { get; set; } = null!;

    }
}
