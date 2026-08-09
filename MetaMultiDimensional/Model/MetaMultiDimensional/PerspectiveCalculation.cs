#nullable enable

namespace MetaMultiDimensional
{
    public sealed class PerspectiveCalculation
    {
        public string Id { get; set; } = string.Empty;

        public MdxCalculation MdxCalculation { get; set; } = null!;

        public Perspective Perspective { get; set; } = null!;

    }
}
