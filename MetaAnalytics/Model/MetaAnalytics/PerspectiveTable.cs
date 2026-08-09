#nullable enable

namespace MetaAnalytics
{
    public sealed class PerspectiveTable
    {
        public string Id { get; set; } = string.Empty;

        public Perspective Perspective { get; set; } = null!;

        public Table Table { get; set; } = null!;

    }
}
