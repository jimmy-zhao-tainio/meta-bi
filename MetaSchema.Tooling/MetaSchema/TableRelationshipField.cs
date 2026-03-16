namespace MetaSchema
{
    public sealed class TableRelationshipField
    {
        public string Id { get; set; } = string.Empty;
        public string Ordinal { get; set; } = string.Empty;
        public string SourceFieldId { get; set; } = string.Empty;
        public Field SourceField { get; set; } = new Field();
        public string TableRelationshipId { get; set; } = string.Empty;
        public TableRelationship TableRelationship { get; set; } = new TableRelationship();
        public string TargetFieldId { get; set; } = string.Empty;
        public Field TargetField { get; set; } = new Field();
    }
}
