#nullable enable

namespace MetaTransformScript
{
    public sealed class WindowDefinitionWindowFrameClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public WindowDefinition WindowDefinition { get; set; } = null!;

        public WindowFrameClause WindowFrameClause { get; set; } = null!;

    }
}
