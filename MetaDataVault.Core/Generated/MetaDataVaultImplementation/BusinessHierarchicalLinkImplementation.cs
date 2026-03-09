namespace MetaDataVaultImplementation
{
    public sealed class BusinessHierarchicalLinkImplementation
    {
        public string Id { get; internal set; } = string.Empty;
        public string ChildHashKeyColumnName { get; internal set; } = string.Empty;
        public string HashKeyColumnName { get; internal set; } = string.Empty;
        public string HashKeyDataTypeId { get; internal set; } = string.Empty;
        public string HashKeyLength { get; internal set; } = string.Empty;
        public string LoadTimestampColumnName { get; internal set; } = string.Empty;
        public string LoadTimestampDataTypeId { get; internal set; } = string.Empty;
        public string LoadTimestampPrecision { get; internal set; } = string.Empty;
        public string ParentHashKeyColumnName { get; internal set; } = string.Empty;
        public string RecordSourceColumnName { get; internal set; } = string.Empty;
        public string RecordSourceDataTypeId { get; internal set; } = string.Empty;
        public string RecordSourceLength { get; internal set; } = string.Empty;
        public string TableNamePattern { get; internal set; } = string.Empty;
    }
}
