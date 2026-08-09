#nullable enable

namespace MetaTransformScript
{
    public sealed class CubeGroupingSpecificationArgumentsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public CubeGroupingSpecification CubeGroupingSpecification { get; set; } = null!;

        public GroupingSpecification GroupingSpecification { get; set; } = null!;

    }
}
