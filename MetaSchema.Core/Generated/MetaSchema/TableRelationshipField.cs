namespace MetaSchema
{
    public sealed class TableRelationshipField
    {
        public string Id { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string SourceFieldId { get; internal set; } = string.Empty;
        public Field SourceField { get; internal set; } = new Field();
        public string TableRelationshipId { get; internal set; } = string.Empty;
        public TableRelationship TableRelationship { get; internal set; } = new TableRelationship();
        public string TargetFieldId { get; internal set; } = string.Empty;
        public Field TargetField { get; internal set; } = new Field();
    }
}
