#nullable enable

namespace MetaSql
{
    public sealed class PrimaryKeyColumn
    {
        public string Id { get; set; } = string.Empty;

        public string? IsDescending { get; set; }

        public string Ordinal { get; set; } = string.Empty;

        public PrimaryKey PrimaryKey { get; set; } = null!;

        public TableColumn TableColumn { get; set; } = null!;

    }
}
