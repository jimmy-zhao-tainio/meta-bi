#nullable enable

namespace MetaTabular
{
    public sealed class TabularCulture
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public TabularModel TabularModel { get; set; } = null!;

    }
}
