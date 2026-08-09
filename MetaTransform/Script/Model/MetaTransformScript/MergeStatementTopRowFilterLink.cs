#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeStatementTopRowFilterLink
    {
        public string Id { get; set; } = string.Empty;

        public MergeStatement MergeStatement { get; set; } = null!;

        public TopRowFilter TopRowFilter { get; set; } = null!;

    }
}
