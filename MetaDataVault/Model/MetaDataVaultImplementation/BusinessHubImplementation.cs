#nullable enable

namespace MetaDataVaultImplementation
{
    public sealed class BusinessHubImplementation
    {
        public string Id { get; set; } = string.Empty;

        public string AuditIdColumnName { get; set; } = string.Empty;

        public string AuditIdDataTypeId { get; set; } = string.Empty;

        public string? AuditIdDefaultExpressionSql { get; set; }

        public string HashKeyColumnName { get; set; } = string.Empty;

        public string HashKeyDataTypeId { get; set; } = string.Empty;

        public string HashKeyLength { get; set; } = string.Empty;

        public string? LoadTimestampColumnName { get; set; }

        public string? LoadTimestampDataTypeId { get; set; }

        public string? LoadTimestampDefaultExpressionSql { get; set; }

        public string? LoadTimestampPrecision { get; set; }

        public string PrimaryKeyNamePattern { get; set; } = string.Empty;

        public string? RecordSourceColumnName { get; set; }

        public string? RecordSourceDataTypeId { get; set; }

        public string? RecordSourceLength { get; set; }

        public string SchemaName { get; set; } = string.Empty;

        public string TableNamePattern { get; set; } = string.Empty;

    }
}
