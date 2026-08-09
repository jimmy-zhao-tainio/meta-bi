#nullable enable

namespace MetaTransformScript
{
    public sealed class HavingClauseSearchConditionLink
    {
        public string Id { get; set; } = string.Empty;

        public BooleanExpression BooleanExpression { get; set; } = null!;

        public HavingClause HavingClause { get; set; } = null!;

    }
}
