#nullable enable

namespace MetaTransformScript
{
    public sealed class WhereClauseSearchConditionLink
    {
        public string Id { get; set; } = string.Empty;

        public BooleanExpression BooleanExpression { get; set; } = null!;

        public WhereClause WhereClause { get; set; } = null!;

    }
}
