namespace MetaDataVaultImplementation
{
    public sealed class BusinessBridgeImplementation
    {
        public string Id { get; set; } = string.Empty;
        public string AuditIdColumnName { get; set; } = string.Empty;
        public string AuditIdDataTypeId { get; set; } = string.Empty;
        public string DepthColumnName { get; set; } = string.Empty;
        public string DepthDataTypeId { get; set; } = string.Empty;
        public string EffectiveFromColumnName { get; set; } = string.Empty;
        public string EffectiveFromDataTypeId { get; set; } = string.Empty;
        public string EffectiveFromPrecision { get; set; } = string.Empty;
        public string EffectiveToColumnName { get; set; } = string.Empty;
        public string EffectiveToDataTypeId { get; set; } = string.Empty;
        public string EffectiveToPrecision { get; set; } = string.Empty;
        public string PathColumnName { get; set; } = string.Empty;
        public string PathDataTypeId { get; set; } = string.Empty;
        public string PathLength { get; set; } = string.Empty;
        public string RelatedHashKeyColumnName { get; set; } = string.Empty;
        public string RelatedHashKeyDataTypeId { get; set; } = string.Empty;
        public string RelatedHashKeyLength { get; set; } = string.Empty;
        public string RootHashKeyColumnName { get; set; } = string.Empty;
        public string RootHashKeyDataTypeId { get; set; } = string.Empty;
        public string RootHashKeyLength { get; set; } = string.Empty;
        public string TableNamePattern { get; set; } = string.Empty;
    }
}
