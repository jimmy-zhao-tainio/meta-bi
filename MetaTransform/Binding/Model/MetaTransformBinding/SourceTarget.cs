#nullable enable

namespace MetaTransformBinding
{
    public sealed class SourceTarget
    {
        public string Id { get; set; } = string.Empty;

        public string? InputRole { get; set; }

        public string Ordinal { get; set; } = string.Empty;

        public Rowset Source { get; set; } = null!;

        public Rowset Target { get; set; } = null!;

    }
}
