#nullable enable

namespace MetaMultiDimensional
{
    public sealed class KpiTranslation
    {
        public string Id { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public string? Description { get; set; }

        public Culture Culture { get; set; } = null!;

        public Kpi Kpi { get; set; } = null!;

    }
}
