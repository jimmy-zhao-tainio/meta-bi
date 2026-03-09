namespace MetaDataVaultImplementation
{
    public sealed class BusinessBridgeImplementation
    {
        public string Id { get; internal set; } = string.Empty;
        public string AuditIdColumnName { get; internal set; } = string.Empty;
        public string AuditIdDataTypeId { get; internal set; } = string.Empty;
        public string DepthColumnName { get; internal set; } = string.Empty;
        public string DepthDataTypeId { get; internal set; } = string.Empty;
        public string EffectiveFromColumnName { get; internal set; } = string.Empty;
        public string EffectiveFromDataTypeId { get; internal set; } = string.Empty;
        public string EffectiveFromPrecision { get; internal set; } = string.Empty;
        public string EffectiveToColumnName { get; internal set; } = string.Empty;
        public string EffectiveToDataTypeId { get; internal set; } = string.Empty;
        public string EffectiveToPrecision { get; internal set; } = string.Empty;
        public string PathColumnName { get; internal set; } = string.Empty;
        public string PathDataTypeId { get; internal set; } = string.Empty;
        public string PathLength { get; internal set; } = string.Empty;
        public string RelatedHashKeyColumnName { get; internal set; } = string.Empty;
        public string RelatedHashKeyDataTypeId { get; internal set; } = string.Empty;
        public string RelatedHashKeyLength { get; internal set; } = string.Empty;
        public string RootHashKeyColumnName { get; internal set; } = string.Empty;
        public string RootHashKeyDataTypeId { get; internal set; } = string.Empty;
        public string RootHashKeyLength { get; internal set; } = string.Empty;
        public string TableNamePattern { get; internal set; } = string.Empty;
    }
}
