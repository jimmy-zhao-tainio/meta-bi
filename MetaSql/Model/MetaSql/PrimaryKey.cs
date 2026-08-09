#nullable enable

namespace MetaSql
{
    public sealed class PrimaryKey
    {
        public string Id { get; set; } = string.Empty;

        public string? IsClustered { get; set; }

        public string Name { get; set; } = string.Empty;

        public Table Table { get; set; } = null!;

    }
}
