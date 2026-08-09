#nullable enable

namespace MetaTransformBinding
{
    public sealed class Column
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public Rowset Rowset { get; set; } = null!;

    }
}
