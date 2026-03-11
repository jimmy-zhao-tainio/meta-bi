namespace MetaRawDataVault
{
    public sealed class SourceTableRelationshipField
    {
        public string Id { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string SourceFieldName { get; internal set; } = string.Empty;
        public string TargetFieldName { get; internal set; } = string.Empty;
        public string SourceFieldId { get; internal set; } = string.Empty;
        public SourceField SourceField { get; internal set; } = new SourceField();
        public string SourceTableRelationshipId { get; internal set; } = string.Empty;
        public SourceTableRelationship SourceTableRelationship { get; internal set; } = new SourceTableRelationship();
    }
}
