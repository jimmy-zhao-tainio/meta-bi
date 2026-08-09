#nullable enable

namespace MetaTransformScript
{
    public sealed class FunctionCallWithinGroupOrderByClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public FunctionCall FunctionCall { get; set; } = null!;

        public OrderByClause OrderByClause { get; set; } = null!;

    }
}
