#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeStatementTargetHintsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public MergeStatement MergeStatement { get; set; } = null!;

        public SqlHint SqlHint { get; set; } = null!;

    }
}
