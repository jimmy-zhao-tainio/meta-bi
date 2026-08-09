#nullable enable

namespace MetaDataWarehouse
{
    public sealed class FactMeasure
    {
        public string Id { get; set; } = string.Empty;

        public string DataTypeId { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? IsNullable { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public Fact Fact { get; set; } = null!;

    }
}
