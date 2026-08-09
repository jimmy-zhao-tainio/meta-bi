#nullable enable

namespace MetaTransformScript
{
    public sealed class QuerySpecification
    {
        public string Id { get; set; } = string.Empty;

        public string? UniqueRowFilter { get; set; }

        public QueryExpression QueryExpression { get; set; } = null!;

    }
}
