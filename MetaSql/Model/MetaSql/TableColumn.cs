#nullable enable

namespace MetaSql
{
    public sealed class TableColumn
    {
        public string Id { get; set; } = string.Empty;

        public string? DefaultExpressionSql { get; set; }

        public string? ExpressionSql { get; set; }

        public string? IdentityIncrement { get; set; }

        public string? IdentitySeed { get; set; }

        public string? IsIdentity { get; set; }

        public string? IsNullable { get; set; }

        public string MetaDataTypeId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public Table Table { get; set; } = null!;

    }
}
