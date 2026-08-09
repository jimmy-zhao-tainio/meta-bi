#nullable enable

namespace MetaAnalytics
{
    public sealed class PerspectiveAttribute
    {
        public string Id { get; set; } = string.Empty;

        public Attribute Attribute { get; set; } = null!;

        public Perspective Perspective { get; set; } = null!;

    }
}
