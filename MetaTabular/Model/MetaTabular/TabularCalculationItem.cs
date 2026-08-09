#nullable enable

namespace MetaTabular
{
    public sealed class TabularCalculationItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Expression { get; set; } = string.Empty;

        public string? FormatStringExpression { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public TabularCalculationGroup TabularCalculationGroup { get; set; } = null!;

    }
}
