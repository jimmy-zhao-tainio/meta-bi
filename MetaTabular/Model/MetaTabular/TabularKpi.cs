#nullable enable

namespace MetaTabular
{
    public sealed class TabularKpi
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? StatusExpression { get; set; }

        public string? StatusGraphic { get; set; }

        public string? TargetExpression { get; set; }

        public string? TrendExpression { get; set; }

        public string? TrendGraphic { get; set; }

        public TabularMeasure BaseMeasure { get; set; } = null!;

        public TabularMeasure? TargetMeasure { get; set; }

    }
}
