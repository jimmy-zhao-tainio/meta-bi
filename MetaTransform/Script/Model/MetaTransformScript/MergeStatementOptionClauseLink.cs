#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeStatementOptionClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public MergeStatement MergeStatement { get; set; } = null!;

        public OptionClause OptionClause { get; set; } = null!;

    }
}
