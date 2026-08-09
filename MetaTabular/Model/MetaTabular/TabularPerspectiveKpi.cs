#nullable enable

namespace MetaTabular
{
    public sealed class TabularPerspectiveKpi
    {
        public string Id { get; set; } = string.Empty;

        public TabularKpi TabularKpi { get; set; } = null!;

        public TabularPerspective TabularPerspective { get; set; } = null!;

    }
}
