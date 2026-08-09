#nullable enable

namespace MetaTransformScript
{
    public sealed class QuerySpecificationWhereClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public QuerySpecification QuerySpecification { get; set; } = null!;

        public WhereClause WhereClause { get; set; } = null!;

    }
}
