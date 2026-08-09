#nullable enable

namespace MetaTransformScript
{
    public sealed class TableSampleClauseRepeatSeedLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public TableSampleClause TableSampleClause { get; set; } = null!;

    }
}
