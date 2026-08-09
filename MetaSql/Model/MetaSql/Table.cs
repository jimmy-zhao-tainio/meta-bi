#nullable enable

namespace MetaSql
{
    public sealed class Table
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Schema Schema { get; set; } = null!;

    }
}
