#nullable enable

namespace MetaSchema
{
    public sealed class TableRelationshipField
    {
        public string Id { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public Field SourceField { get; set; } = null!;

        public TableRelationship TableRelationship { get; set; } = null!;

        public Field TargetField { get; set; } = null!;

    }
}
