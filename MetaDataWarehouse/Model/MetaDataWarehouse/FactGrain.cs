#nullable enable

namespace MetaDataWarehouse
{
    public sealed class FactGrain
    {
        public string Id { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Fact Fact { get; set; } = null!;

    }
}
