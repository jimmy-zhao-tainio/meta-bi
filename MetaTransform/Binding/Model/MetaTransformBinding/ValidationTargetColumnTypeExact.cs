#nullable enable

namespace MetaTransformBinding
{
    public sealed class ValidationTargetColumnTypeExact
    {
        public string Id { get; set; } = string.Empty;

        public string SourceMetaDataTypeId { get; set; } = string.Empty;

        public string TargetMetaDataTypeId { get; set; } = string.Empty;

        public ValidationTargetColumnLink ValidationTargetColumnLink { get; set; } = null!;

    }
}
