#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeStatementTargetLink
    {
        public string Id { get; set; } = string.Empty;

        public MergeStatement MergeStatement { get; set; } = null!;

        public SchemaObjectName SchemaObjectName { get; set; } = null!;

    }
}
