#nullable enable

namespace MetaTransformBinding
{
    public sealed class ValidationTargetColumnLink
    {
        public string Id { get; set; } = string.Empty;

        public string MetaSchemaFieldId { get; set; } = string.Empty;

        public Column Column { get; set; } = null!;

        public ValidationTargetRowsetLink ValidationTargetRowsetLink { get; set; } = null!;

    }
}
