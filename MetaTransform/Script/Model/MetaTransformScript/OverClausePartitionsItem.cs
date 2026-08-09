#nullable enable

namespace MetaTransformScript
{
    public sealed class OverClausePartitionsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public OverClause OverClause { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
