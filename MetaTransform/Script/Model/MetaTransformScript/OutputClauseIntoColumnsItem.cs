#nullable enable

namespace MetaTransformScript
{
    public sealed class OutputClauseIntoColumnsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public Identifier Identifier { get; set; } = null!;

        public OutputClause OutputClause { get; set; } = null!;

    }
}
