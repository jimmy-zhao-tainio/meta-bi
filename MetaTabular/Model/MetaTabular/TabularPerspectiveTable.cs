#nullable enable

namespace MetaTabular
{
    public sealed class TabularPerspectiveTable
    {
        public string Id { get; set; } = string.Empty;

        public TabularPerspective TabularPerspective { get; set; } = null!;

        public TabularTable TabularTable { get; set; } = null!;

    }
}
