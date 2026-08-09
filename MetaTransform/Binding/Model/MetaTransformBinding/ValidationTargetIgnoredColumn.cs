#nullable enable

namespace MetaTransformBinding
{
    public sealed class ValidationTargetIgnoredColumn
    {
        public string Id { get; set; } = string.Empty;

        public string MetaSchemaFieldId { get; set; } = string.Empty;

        public ValidationTargetRowsetLink ValidationTargetRowsetLink { get; set; } = null!;

    }
}
