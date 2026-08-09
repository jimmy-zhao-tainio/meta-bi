#nullable enable

namespace MetaPipeline
{
    public sealed class RowStreamColumn
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public RowStream RowStream { get; set; } = null!;

    }
}
