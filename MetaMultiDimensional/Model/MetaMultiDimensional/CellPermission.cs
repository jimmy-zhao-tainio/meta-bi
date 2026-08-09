#nullable enable

namespace MetaMultiDimensional
{
    public sealed class CellPermission
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Expression { get; set; } = string.Empty;

        public Cube Cube { get; set; } = null!;

        public SecurityRole SecurityRole { get; set; } = null!;

    }
}
