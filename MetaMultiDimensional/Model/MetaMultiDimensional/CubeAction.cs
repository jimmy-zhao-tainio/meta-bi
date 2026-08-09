#nullable enable

namespace MetaMultiDimensional
{
    public sealed class CubeAction
    {
        public string Id { get; set; } = string.Empty;

        public string ActionType { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public string? Description { get; set; }

        public string Expression { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Target { get; set; }

        public string TargetKind { get; set; } = string.Empty;

        public Cube Cube { get; set; } = null!;

    }
}
