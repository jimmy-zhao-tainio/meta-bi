#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeStatementSourceLink
    {
        public string Id { get; set; } = string.Empty;

        public MergeStatement MergeStatement { get; set; } = null!;

        public TableReference TableReference { get; set; } = null!;

    }
}
