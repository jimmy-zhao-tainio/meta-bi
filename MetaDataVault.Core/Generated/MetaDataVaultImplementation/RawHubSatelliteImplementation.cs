namespace MetaDataVaultImplementation
{
    public sealed class RawHubSatelliteImplementation
    {
        public string Id { get; internal set; } = string.Empty;
        public string HashDiffColumnName { get; internal set; } = string.Empty;
        public string HashDiffDataTypeId { get; internal set; } = string.Empty;
        public string HashDiffLength { get; internal set; } = string.Empty;
        public string LoadTimestampColumnName { get; internal set; } = string.Empty;
        public string LoadTimestampDataTypeId { get; internal set; } = string.Empty;
        public string LoadTimestampPrecision { get; internal set; } = string.Empty;
        public string ParentHashKeyColumnName { get; internal set; } = string.Empty;
        public string ParentHashKeyDataTypeId { get; internal set; } = string.Empty;
        public string ParentHashKeyLength { get; internal set; } = string.Empty;
        public string RecordSourceColumnName { get; internal set; } = string.Empty;
        public string RecordSourceDataTypeId { get; internal set; } = string.Empty;
        public string RecordSourceLength { get; internal set; } = string.Empty;
        public string TableNamePattern { get; internal set; } = string.Empty;
    }
}
