#nullable enable

namespace MetaSql
{
    public sealed class Database
    {
        public string Id { get; set; } = string.Empty;

        public string? Collation { get; set; }

        public string Name { get; set; } = string.Empty;

    }
}
