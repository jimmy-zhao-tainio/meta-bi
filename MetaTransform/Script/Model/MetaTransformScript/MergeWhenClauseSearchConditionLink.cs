#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeWhenClauseSearchConditionLink
    {
        public string Id { get; set; } = string.Empty;

        public BooleanExpression BooleanExpression { get; set; } = null!;

        public MergeWhenClause MergeWhenClause { get; set; } = null!;

    }
}
