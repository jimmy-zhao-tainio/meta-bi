#nullable enable

namespace MetaTransformScript
{
    public sealed class WindowClauseWindowDefinitionItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public WindowClause WindowClause { get; set; } = null!;

        public WindowDefinition WindowDefinition { get; set; } = null!;

    }
}
