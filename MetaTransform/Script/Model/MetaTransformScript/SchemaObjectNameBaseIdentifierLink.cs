#nullable enable

namespace MetaTransformScript
{
    public sealed class SchemaObjectNameBaseIdentifierLink
    {
        public string Id { get; set; } = string.Empty;

        public Identifier Identifier { get; set; } = null!;

        public SchemaObjectName SchemaObjectName { get; set; } = null!;

    }
}
