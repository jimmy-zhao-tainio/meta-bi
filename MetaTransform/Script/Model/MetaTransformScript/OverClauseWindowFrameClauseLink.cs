#nullable enable

namespace MetaTransformScript
{
    public sealed class OverClauseWindowFrameClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public OverClause OverClause { get; set; } = null!;

        public WindowFrameClause WindowFrameClause { get; set; } = null!;

    }
}
