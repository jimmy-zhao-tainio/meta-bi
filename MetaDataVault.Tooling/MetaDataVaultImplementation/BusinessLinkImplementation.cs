namespace MetaDataVaultImplementation
{
    public sealed class BusinessLinkImplementation
    {
        public string Id { get; set; } = string.Empty;
        public string AuditIdColumnName { get; set; } = string.Empty;
        public string AuditIdDataTypeId { get; set; } = string.Empty;
        public string EndHashKeyColumnPattern { get; set; } = string.Empty;
        public string HashKeyColumnName { get; set; } = string.Empty;
        public string HashKeyDataTypeId { get; set; } = string.Empty;
        public string HashKeyLength { get; set; } = string.Empty;
        public string LoadTimestampColumnName { get; set; } = string.Empty;
        public string LoadTimestampDataTypeId { get; set; } = string.Empty;
        public string LoadTimestampPrecision { get; set; } = string.Empty;
        public string RecordSourceColumnName { get; set; } = string.Empty;
        public string RecordSourceDataTypeId { get; set; } = string.Empty;
        public string RecordSourceLength { get; set; } = string.Empty;
        public string TableNamePattern { get; set; } = string.Empty;
    }
}
