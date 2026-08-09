#nullable enable

namespace MetaTransformScript
{
    public sealed class WindowDefinitionOrderByClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public OrderByClause OrderByClause { get; set; } = null!;

        public WindowDefinition WindowDefinition { get; set; } = null!;

    }
}
