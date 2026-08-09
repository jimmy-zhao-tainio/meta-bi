#nullable enable

namespace MetaSchema
{
    public sealed class Field
    {
        public string Id { get; set; } = string.Empty;

        public string? IdentityIncrement { get; set; }

        public string? IdentitySeed { get; set; }

        public string? IsIdentity { get; set; }

        public string? IsNullable { get; set; }

        public string MetaDataTypeId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public SchemaObject SchemaObject { get; set; } = null!;

    }
}
