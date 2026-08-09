#nullable enable

namespace MetaSchema
{
    public sealed class TableRelationship
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Table SourceTable { get; set; } = null!;

        public Table TargetTable { get; set; } = null!;

    }
}
