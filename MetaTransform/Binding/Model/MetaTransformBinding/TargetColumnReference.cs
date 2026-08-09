#nullable enable

namespace MetaTransformBinding
{
    public sealed class TargetColumnReference
    {
        public string Id { get; set; } = string.Empty;

        public string MetaSchemaFieldId { get; set; } = string.Empty;

        public string MetaTransformScriptColumnReferenceId { get; set; } = string.Empty;

        public Column Column { get; set; } = null!;

        public TransformBindingTarget TransformBindingTarget { get; set; } = null!;

    }
}
