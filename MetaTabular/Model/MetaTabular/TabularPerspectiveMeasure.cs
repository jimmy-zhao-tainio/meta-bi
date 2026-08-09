#nullable enable

namespace MetaTabular
{
    public sealed class TabularPerspectiveMeasure
    {
        public string Id { get; set; } = string.Empty;

        public TabularMeasure TabularMeasure { get; set; } = null!;

        public TabularPerspective TabularPerspective { get; set; } = null!;

    }
}
