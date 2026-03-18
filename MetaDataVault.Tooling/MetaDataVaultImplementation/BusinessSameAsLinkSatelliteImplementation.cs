namespace MetaDataVaultImplementation
{
    public sealed class BusinessSameAsLinkSatelliteImplementation
    {
        public string Id { get; set; } = string.Empty;
        public string AuditIdColumnName { get; set; } = string.Empty;
        public string AuditIdDataTypeId { get; set; } = string.Empty;
        public string HashDiffColumnName { get; set; } = string.Empty;
        public string HashDiffDataTypeId { get; set; } = string.Empty;
        public string HashDiffLength { get; set; } = string.Empty;
        public string LoadTimestampColumnName { get; set; } = string.Empty;
        public string LoadTimestampDataTypeId { get; set; } = string.Empty;
        public string LoadTimestampPrecision { get; set; } = string.Empty;
        public string ParentForeignKeyNamePattern { get; set; } = string.Empty;
        public string ParentHashKeyColumnName { get; set; } = string.Empty;
        public string ParentHashKeyDataTypeId { get; set; } = string.Empty;
        public string ParentHashKeyLength { get; set; } = string.Empty;
        public string RecordSourceColumnName { get; set; } = string.Empty;
        public string RecordSourceDataTypeId { get; set; } = string.Empty;
        public string RecordSourceLength { get; set; } = string.Empty;
        public string TableNamePattern { get; set; } = string.Empty;
    }
}
