#nullable enable

namespace MetaSql
{
    public sealed class IndexColumn
    {
        public string Id { get; set; } = string.Empty;

        public string? IsDescending { get; set; }

        public string? IsIncluded { get; set; }

        public string Ordinal { get; set; } = string.Empty;

        public Index Index { get; set; } = null!;

        public TableColumn TableColumn { get; set; } = null!;

    }
}
