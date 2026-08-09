#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeStatementWhenClausesItem
    {
        public string Id { get; set; } = string.Empty;

        public MergeStatement MergeStatement { get; set; } = null!;

        public MergeWhenClause MergeWhenClause { get; set; } = null!;

        public MergeStatementWhenClausesItem? PreviousMergeWhenClause { get; set; }

    }
}
