#nullable enable

namespace MetaAnalytics
{
    public sealed class HierarchyLevel
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public Attribute Attribute { get; set; } = null!;

        public Hierarchy Hierarchy { get; set; } = null!;

    }
}
