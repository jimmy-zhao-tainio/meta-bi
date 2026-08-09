#nullable enable

namespace MetaTransformScript
{
    public sealed class WindowFrameClauseBottomLink
    {
        public string Id { get; set; } = string.Empty;

        public WindowDelimiter WindowDelimiter { get; set; } = null!;

        public WindowFrameClause WindowFrameClause { get; set; } = null!;

    }
}
