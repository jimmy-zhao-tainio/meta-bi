#nullable enable

namespace MetaSql
{
    public sealed class Schema
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Database Database { get; set; } = null!;

    }
}
