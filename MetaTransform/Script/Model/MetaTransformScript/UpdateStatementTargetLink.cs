#nullable enable

namespace MetaTransformScript
{
    public sealed class UpdateStatementTargetLink
    {
        public string Id { get; set; } = string.Empty;

        public SchemaObjectName SchemaObjectName { get; set; } = null!;

        public UpdateStatement UpdateStatement { get; set; } = null!;

    }
}
