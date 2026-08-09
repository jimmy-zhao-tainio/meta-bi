#nullable enable

namespace MetaTabular
{
    public sealed class TabularPerspectiveCalculationGroup
    {
        public string Id { get; set; } = string.Empty;

        public TabularCalculationGroup TabularCalculationGroup { get; set; } = null!;

        public TabularPerspective TabularPerspective { get; set; } = null!;

    }
}
