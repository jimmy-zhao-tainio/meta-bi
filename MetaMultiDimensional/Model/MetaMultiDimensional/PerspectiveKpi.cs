#nullable enable

namespace MetaMultiDimensional
{
    public sealed class PerspectiveKpi
    {
        public string Id { get; set; } = string.Empty;

        public Kpi Kpi { get; set; } = null!;

        public Perspective Perspective { get; set; } = null!;

    }
}
