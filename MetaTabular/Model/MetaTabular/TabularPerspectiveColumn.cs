#nullable enable

namespace MetaTabular
{
    public sealed class TabularPerspectiveColumn
    {
        public string Id { get; set; } = string.Empty;

        public TabularColumn TabularColumn { get; set; } = null!;

        public TabularPerspective TabularPerspective { get; set; } = null!;

    }
}
