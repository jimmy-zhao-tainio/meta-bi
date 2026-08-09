#nullable enable

namespace MetaDataWarehouse
{
    public sealed class MiniDimension
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? RoleName { get; set; }

        public Dimension ProfileDimension { get; set; } = null!;

        public Dimension SourceDimension { get; set; } = null!;

    }
}
