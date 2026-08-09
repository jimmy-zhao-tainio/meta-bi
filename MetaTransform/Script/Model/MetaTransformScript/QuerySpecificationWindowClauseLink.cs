#nullable enable

namespace MetaTransformScript
{
    public sealed class QuerySpecificationWindowClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public QuerySpecification QuerySpecification { get; set; } = null!;

        public WindowClause WindowClause { get; set; } = null!;

    }
}
