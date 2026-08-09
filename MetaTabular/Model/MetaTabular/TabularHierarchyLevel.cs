#nullable enable

namespace MetaTabular
{
    public sealed class TabularHierarchyLevel
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public TabularColumn TabularColumn { get; set; } = null!;

        public TabularHierarchy TabularHierarchy { get; set; } = null!;

    }
}
