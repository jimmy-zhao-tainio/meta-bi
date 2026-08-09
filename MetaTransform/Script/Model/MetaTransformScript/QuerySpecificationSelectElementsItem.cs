#nullable enable

namespace MetaTransformScript
{
    public sealed class QuerySpecificationSelectElementsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public QuerySpecification QuerySpecification { get; set; } = null!;

        public SelectElement SelectElement { get; set; } = null!;

    }
}
