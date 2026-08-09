#nullable enable

namespace MetaTransformScript
{
    public sealed class OutputClauseSelectElementsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public OutputClause OutputClause { get; set; } = null!;

        public SelectElement SelectElement { get; set; } = null!;

    }
}
