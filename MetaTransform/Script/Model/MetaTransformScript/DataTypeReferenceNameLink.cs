#nullable enable

namespace MetaTransformScript
{
    public sealed class DataTypeReferenceNameLink
    {
        public string Id { get; set; } = string.Empty;

        public DataTypeReference DataTypeReference { get; set; } = null!;

        public SchemaObjectName SchemaObjectName { get; set; } = null!;

    }
}
