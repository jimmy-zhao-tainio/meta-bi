#nullable enable

namespace MetaTabular
{
    public sealed class TabularKpiTranslation
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TabularCulture TabularCulture { get; set; } = null!;

        public TabularKpi TabularKpi { get; set; } = null!;

    }
}
