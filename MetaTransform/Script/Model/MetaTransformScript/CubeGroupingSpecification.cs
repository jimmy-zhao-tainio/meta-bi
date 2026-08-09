#nullable enable

namespace MetaTransformScript
{
    public sealed class CubeGroupingSpecification
    {
        public string Id { get; set; } = string.Empty;

        public GroupingSpecification GroupingSpecification { get; set; } = null!;

    }
}
