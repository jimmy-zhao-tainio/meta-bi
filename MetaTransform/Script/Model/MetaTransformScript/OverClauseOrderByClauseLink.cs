#nullable enable

namespace MetaTransformScript
{
    public sealed class OverClauseOrderByClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public OrderByClause OrderByClause { get; set; } = null!;

        public OverClause OverClause { get; set; } = null!;

    }
}
