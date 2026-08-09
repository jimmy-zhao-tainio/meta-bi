#nullable enable

namespace MetaTransformBinding
{
    public sealed class ColumnReference
    {
        public string Id { get; set; } = string.Empty;

        public string MetaTransformScriptColumnReferenceId { get; set; } = string.Empty;

        public Column Column { get; set; } = null!;

        public TableSource TableSource { get; set; } = null!;

        public TransformBinding TransformBinding { get; set; } = null!;

    }
}
