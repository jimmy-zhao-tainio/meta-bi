namespace MetaDataVaultImplementation
{
    public sealed class BusinessPointInTimeImplementation
    {
        public string Id { get; internal set; } = string.Empty;
        public string AuditIdColumnName { get; internal set; } = string.Empty;
        public string AuditIdDataTypeId { get; internal set; } = string.Empty;
        public string ParentHashKeyColumnName { get; internal set; } = string.Empty;
        public string ParentHashKeyDataTypeId { get; internal set; } = string.Empty;
        public string ParentHashKeyLength { get; internal set; } = string.Empty;
        public string SatelliteReferenceColumnNamePattern { get; internal set; } = string.Empty;
        public string SatelliteReferenceDataTypeId { get; internal set; } = string.Empty;
        public string SatelliteReferencePrecision { get; internal set; } = string.Empty;
        public string SnapshotTimestampColumnName { get; internal set; } = string.Empty;
        public string SnapshotTimestampDataTypeId { get; internal set; } = string.Empty;
        public string SnapshotTimestampPrecision { get; internal set; } = string.Empty;
        public string TableNamePattern { get; internal set; } = string.Empty;
    }
}
