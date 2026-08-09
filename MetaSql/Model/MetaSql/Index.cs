#nullable enable

namespace MetaSql
{
    public sealed class Index
    {
        public string Id { get; set; } = string.Empty;

        public string? FilterSql { get; set; }

        public string? IsClustered { get; set; }

        public string? IsUnique { get; set; }

        public string Name { get; set; } = string.Empty;

        public Table Table { get; set; } = null!;

    }
}
