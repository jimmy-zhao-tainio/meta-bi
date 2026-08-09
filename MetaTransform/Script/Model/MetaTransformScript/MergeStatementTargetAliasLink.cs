#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeStatementTargetAliasLink
    {
        public string Id { get; set; } = string.Empty;

        public Identifier Identifier { get; set; } = null!;

        public MergeStatement MergeStatement { get; set; } = null!;

    }
}
