namespace MetaDataVaultImplementation
{
    public sealed class BusinessPointInTimeImplementation
    {
        public string Id { get; set; } = string.Empty;
        public string AnchorHubForeignKeyNamePattern { get; set; } = string.Empty;
        public string AuditIdColumnName { get; set; } = string.Empty;
        public string AuditIdDataTypeId { get; set; } = string.Empty;
        public string ParentHashKeyColumnName { get; set; } = string.Empty;
        public string ParentHashKeyDataTypeId { get; set; } = string.Empty;
        public string ParentHashKeyLength { get; set; } = string.Empty;
        public string SatelliteReferenceColumnNamePattern { get; set; } = string.Empty;
        public string SatelliteReferenceDataTypeId { get; set; } = string.Empty;
        public string SatelliteReferencePrecision { get; set; } = string.Empty;
        public string SnapshotTimestampColumnName { get; set; } = string.Empty;
        public string SnapshotTimestampDataTypeId { get; set; } = string.Empty;
        public string SnapshotTimestampPrecision { get; set; } = string.Empty;
        public string TableNamePattern { get; set; } = string.Empty;
    }
}
