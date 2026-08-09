#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeStatementOutputClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public MergeStatement MergeStatement { get; set; } = null!;

        public OutputClause OutputClause { get; set; } = null!;

    }
}
