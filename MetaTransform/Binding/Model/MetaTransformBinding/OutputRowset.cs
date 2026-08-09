#nullable enable

namespace MetaTransformBinding
{
    public sealed class OutputRowset
    {
        public string Id { get; set; } = string.Empty;

        public Rowset Rowset { get; set; } = null!;

        public TransformBinding TransformBinding { get; set; } = null!;

    }
}
