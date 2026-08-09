#nullable enable

namespace MetaTabular
{
    public sealed class TabularSecurityRole
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Permission { get; set; } = string.Empty;

        public TabularModel TabularModel { get; set; } = null!;

    }
}
