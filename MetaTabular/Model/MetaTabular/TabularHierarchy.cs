#nullable enable

namespace MetaTabular
{
    public sealed class TabularHierarchy
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? DisplayFolder { get; set; }

        public string? IsHidden { get; set; }

        public string Name { get; set; } = string.Empty;

        public TabularTable TabularTable { get; set; } = null!;

    }
}
