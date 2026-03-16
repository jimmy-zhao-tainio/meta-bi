namespace MetaRawDataVault
{
    public sealed class SourceTableRelationshipField
    {
        public string Id { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string SourceFieldId { get; set; } = string.Empty;
        public SourceField SourceField { get; set; } = new SourceField();
        public string SourceTableRelationshipId { get; set; } = string.Empty;
        public SourceTableRelationship SourceTableRelationship { get; set; } = new SourceTableRelationship();
        public string TargetFieldId { get; set; } = string.Empty;
        public SourceField TargetField { get; set; } = new SourceField();
    }
}
