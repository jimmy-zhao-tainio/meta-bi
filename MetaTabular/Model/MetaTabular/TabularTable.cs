#nullable enable

namespace MetaTabular
{
    public sealed class TabularTable
    {
        public string Id { get; set; } = string.Empty;

        public string? DataCategory { get; set; }

        public string? Description { get; set; }

        public string? IsHidden { get; set; }

        public string Name { get; set; } = string.Empty;

        public TabularModel TabularModel { get; set; } = null!;

    }
}
