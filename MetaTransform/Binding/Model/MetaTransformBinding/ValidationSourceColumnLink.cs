#nullable enable

namespace MetaTransformBinding
{
    public sealed class ValidationSourceColumnLink
    {
        public string Id { get; set; } = string.Empty;

        public string MetaSchemaFieldId { get; set; } = string.Empty;

        public Column Column { get; set; } = null!;

        public ValidationSourceRowsetLink ValidationSourceRowsetLink { get; set; } = null!;

    }
}
