#nullable enable

namespace MetaTransformScript
{
    public sealed class QuerySpecificationHavingClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public HavingClause HavingClause { get; set; } = null!;

        public QuerySpecification QuerySpecification { get; set; } = null!;

    }
}
