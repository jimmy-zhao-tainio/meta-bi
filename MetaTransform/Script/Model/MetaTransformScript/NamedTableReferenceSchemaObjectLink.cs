#nullable enable

namespace MetaTransformScript
{
    public sealed class NamedTableReferenceSchemaObjectLink
    {
        public string Id { get; set; } = string.Empty;

        public NamedTableReference NamedTableReference { get; set; } = null!;

        public SchemaObjectName SchemaObjectName { get; set; } = null!;

    }
}
