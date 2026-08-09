#nullable enable

namespace MetaTransformScript
{
    public sealed class TruncateStatementTargetLink
    {
        public string Id { get; set; } = string.Empty;

        public SchemaObjectName SchemaObjectName { get; set; } = null!;

        public TruncateStatement TruncateStatement { get; set; } = null!;

    }
}
