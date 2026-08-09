#nullable enable

namespace MetaTransformScript
{
    public sealed class GroupByClauseGroupingSpecificationsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public GroupByClause GroupByClause { get; set; } = null!;

        public GroupingSpecification GroupingSpecification { get; set; } = null!;

    }
}
