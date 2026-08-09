#nullable enable

namespace MetaDataWarehouseImplementation
{
    public sealed class PlatformColumnImplementation
    {
        public string Id { get; set; } = string.Empty;

        public string AppliesToBridgeTables { get; set; } = string.Empty;

        public string AppliesToDimensionTables { get; set; } = string.Empty;

        public string AppliesToFactTables { get; set; } = string.Empty;

        public string ColumnName { get; set; } = string.Empty;

        public string DataTypeId { get; set; } = string.Empty;

        public string? DefaultExpressionSql { get; set; }

        public string? IsNullable { get; set; }

        public string? Length { get; set; }

        public string Ordinal { get; set; } = string.Empty;

        public string? Precision { get; set; }

        public string? Scale { get; set; }

    }
}
