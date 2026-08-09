#nullable enable

namespace MetaAnalytics
{
    public sealed class PerspectiveHierarchy
    {
        public string Id { get; set; } = string.Empty;

        public Hierarchy Hierarchy { get; set; } = null!;

        public Perspective Perspective { get; set; } = null!;

    }
}
