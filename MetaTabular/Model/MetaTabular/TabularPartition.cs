#nullable enable

namespace MetaTabular
{
    public sealed class TabularPartition
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Expression { get; set; }

        public string? Mode { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public TabularDataSource? TabularDataSource { get; set; }

        public TabularTable TabularTable { get; set; } = null!;

    }
}
