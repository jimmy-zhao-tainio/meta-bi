#nullable enable

namespace MetaMultiDimensional
{
    public sealed class SecurityRole
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Permission { get; set; } = string.Empty;

        public MultiDimensionalDatabase MultiDimensionalDatabase { get; set; } = null!;

    }
}
