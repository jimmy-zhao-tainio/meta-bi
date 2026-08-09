#nullable enable

namespace MetaDataWarehouse
{
    public sealed class Fact
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Name { get; set; } = string.Empty;

        public Warehouse Warehouse { get; set; } = null!;

    }
}
