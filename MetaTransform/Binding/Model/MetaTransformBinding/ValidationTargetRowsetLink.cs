#nullable enable

namespace MetaTransformBinding
{
    public sealed class ValidationTargetRowsetLink
    {
        public string Id { get; set; } = string.Empty;

        public string MetaSchemaTableId { get; set; } = string.Empty;

        public Rowset Rowset { get; set; } = null!;

        public TransformBindingTarget TransformBindingTarget { get; set; } = null!;

        public Validation Validation { get; set; } = null!;

    }
}
