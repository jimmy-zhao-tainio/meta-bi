#nullable enable

namespace MetaTransformBinding
{
    public sealed class TableSource
    {
        public string Id { get; set; } = string.Empty;

        public string ExposedName { get; set; } = string.Empty;

        public string MetaTransformScriptTableReferenceId { get; set; } = string.Empty;

        public Rowset Rowset { get; set; } = null!;

        public TransformBinding TransformBinding { get; set; } = null!;

    }
}
