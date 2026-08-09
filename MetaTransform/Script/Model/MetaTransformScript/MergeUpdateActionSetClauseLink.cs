#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeUpdateActionSetClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public MergeUpdateAction MergeUpdateAction { get; set; } = null!;

        public SetClause SetClause { get; set; } = null!;

    }
}
