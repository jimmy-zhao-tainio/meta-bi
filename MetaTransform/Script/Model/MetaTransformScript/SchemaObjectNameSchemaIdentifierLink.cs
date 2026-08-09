#nullable enable

namespace MetaTransformScript
{
    public sealed class SchemaObjectNameSchemaIdentifierLink
    {
        public string Id { get; set; } = string.Empty;

        public Identifier Identifier { get; set; } = null!;

        public SchemaObjectName SchemaObjectName { get; set; } = null!;

    }
}
