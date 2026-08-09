#nullable enable

namespace MetaTransformScript
{
    public sealed class QuerySpecificationTopRowFilterLink
    {
        public string Id { get; set; } = string.Empty;

        public QuerySpecification QuerySpecification { get; set; } = null!;

        public TopRowFilter TopRowFilter { get; set; } = null!;

    }
}
