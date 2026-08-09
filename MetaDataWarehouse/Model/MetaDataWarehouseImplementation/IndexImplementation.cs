#nullable enable

namespace MetaDataWarehouseImplementation
{
    public sealed class IndexImplementation
    {
        public string Id { get; set; } = string.Empty;

        public string AppliesToBridgeTables { get; set; } = string.Empty;

        public string AppliesToDimensionTables { get; set; } = string.Empty;

        public string AppliesToFactTables { get; set; } = string.Empty;

        public string ColumnNamePattern { get; set; } = string.Empty;

        public string? FilterSql { get; set; }

        public string? IsClustered { get; set; }

        public string? IsUnique { get; set; }

        public string NamePattern { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

    }
}
