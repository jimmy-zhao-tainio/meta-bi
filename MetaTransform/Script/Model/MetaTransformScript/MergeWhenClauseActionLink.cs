#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeWhenClauseActionLink
    {
        public string Id { get; set; } = string.Empty;

        public MergeAction MergeAction { get; set; } = null!;

        public MergeWhenClause MergeWhenClause { get; set; } = null!;

    }
}
