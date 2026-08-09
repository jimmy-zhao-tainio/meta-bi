#nullable enable

namespace MetaTransformScript
{
    public sealed class DeleteStatementTargetLink
    {
        public string Id { get; set; } = string.Empty;

        public DeleteStatement DeleteStatement { get; set; } = null!;

        public SchemaObjectName SchemaObjectName { get; set; } = null!;

    }
}
