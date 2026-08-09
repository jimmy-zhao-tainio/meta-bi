#nullable enable

namespace MetaTabular
{
    public sealed class TabularPerspectiveHierarchy
    {
        public string Id { get; set; } = string.Empty;

        public TabularHierarchy TabularHierarchy { get; set; } = null!;

        public TabularPerspective TabularPerspective { get; set; } = null!;

    }
}
