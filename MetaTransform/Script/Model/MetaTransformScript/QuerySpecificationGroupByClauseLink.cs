#nullable enable

namespace MetaTransformScript
{
    public sealed class QuerySpecificationGroupByClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public GroupByClause GroupByClause { get; set; } = null!;

        public QuerySpecification QuerySpecification { get; set; } = null!;

    }
}
