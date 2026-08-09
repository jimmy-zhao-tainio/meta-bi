#nullable enable

namespace MetaMultiDimensional
{
    public sealed class Kpi
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? GoalExpression { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? StatusExpression { get; set; }

        public string? StatusGraphic { get; set; }

        public string? TrendExpression { get; set; }

        public string? TrendGraphic { get; set; }

        public string? ValueExpression { get; set; }

        public Measure? AssociatedMeasure { get; set; }

        public Cube Cube { get; set; } = null!;

    }
}
