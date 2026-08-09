#nullable enable

namespace MetaMultiDimensional
{
    public sealed class DimensionPermission
    {
        public string Id { get; set; } = string.Empty;

        public string? AllowedSetExpression { get; set; }

        public string? DefaultMemberExpression { get; set; }

        public string? DeniedSetExpression { get; set; }

        public string? Description { get; set; }

        public string? VisualTotals { get; set; }

        public DimensionAttribute DimensionAttribute { get; set; } = null!;

        public Dimension Dimension { get; set; } = null!;

        public SecurityRole SecurityRole { get; set; } = null!;

    }
}
