#nullable enable

namespace MetaDataWarehouseImplementation
{
    public sealed class FactTableImplementation
    {
        public string Id { get; set; } = string.Empty;

        public string DegenerateDimensionColumnPattern { get; set; } = string.Empty;

        public string DimensionForeignKeyNamePattern { get; set; } = string.Empty;

        public string DimensionKeyColumnPattern { get; set; } = string.Empty;

        public string MeasureColumnPattern { get; set; } = string.Empty;

        public string? PrimaryKeyNamePattern { get; set; }

        public string SchemaName { get; set; } = string.Empty;

        public string TableNamePattern { get; set; } = string.Empty;

    }
}
