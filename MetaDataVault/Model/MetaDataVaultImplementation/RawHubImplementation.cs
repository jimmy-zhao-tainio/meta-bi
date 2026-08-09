#nullable enable

namespace MetaDataVaultImplementation
{
    public sealed class RawHubImplementation
    {
        public string Id { get; set; } = string.Empty;

        public string AuditIdColumnName { get; set; } = string.Empty;

        public string AuditIdDataTypeId { get; set; } = string.Empty;

        public string? AuditIdDefaultExpressionSql { get; set; }

        public string HashKeyColumnName { get; set; } = string.Empty;

        public string HashKeyDataTypeId { get; set; } = string.Empty;

        public string HashKeyLength { get; set; } = string.Empty;

        public string LoadTimestampColumnName { get; set; } = string.Empty;

        public string LoadTimestampDataTypeId { get; set; } = string.Empty;

        public string? LoadTimestampDefaultExpressionSql { get; set; }

        public string LoadTimestampPrecision { get; set; } = string.Empty;

        public string PrimaryKeyNamePattern { get; set; } = string.Empty;

        public string RecordSourceColumnName { get; set; } = string.Empty;

        public string RecordSourceDataTypeId { get; set; } = string.Empty;

        public string RecordSourceLength { get; set; } = string.Empty;

        public string SchemaName { get; set; } = string.Empty;

        public string TableNamePattern { get; set; } = string.Empty;

    }
}
