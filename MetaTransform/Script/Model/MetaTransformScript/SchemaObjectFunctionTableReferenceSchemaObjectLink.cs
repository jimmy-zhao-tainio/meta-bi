#nullable enable

namespace MetaTransformScript
{
    public sealed class SchemaObjectFunctionTableReferenceSchemaObjectLink
    {
        public string Id { get; set; } = string.Empty;

        public SchemaObjectFunctionTableReference SchemaObjectFunctionTableReference { get; set; } = null!;

        public SchemaObjectName SchemaObjectName { get; set; } = null!;

    }
}
