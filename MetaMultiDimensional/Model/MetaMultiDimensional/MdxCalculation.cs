#nullable enable

namespace MetaMultiDimensional
{
    public sealed class MdxCalculation
    {
        public string Id { get; set; } = string.Empty;

        public string CalculationKind { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? DisplayFolder { get; set; }

        public string Expression { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? SolveOrder { get; set; }

        public Cube Cube { get; set; } = null!;

    }
}
