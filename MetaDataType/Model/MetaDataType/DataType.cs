#nullable enable

namespace MetaDataType
{
    public sealed class DataType
    {
        public string Id { get; set; } = string.Empty;

        public string? Category { get; set; }

        public string? Description { get; set; }

        public string? IsCanonical { get; set; }

        public string Name { get; set; } = string.Empty;

        public DataTypeSystem DataTypeSystem { get; set; } = null!;

    }
}
