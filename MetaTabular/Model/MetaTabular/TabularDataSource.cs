#nullable enable

namespace MetaTabular
{
    public sealed class TabularDataSource
    {
        public string Id { get; set; } = string.Empty;

        public string? ConnectionReference { get; set; }

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Provider { get; set; }

        public TabularModel TabularModel { get; set; } = null!;

    }
}
