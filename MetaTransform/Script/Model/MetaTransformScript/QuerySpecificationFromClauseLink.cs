#nullable enable

namespace MetaTransformScript
{
    public sealed class QuerySpecificationFromClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public FromClause FromClause { get; set; } = null!;

        public QuerySpecification QuerySpecification { get; set; } = null!;

    }
}
