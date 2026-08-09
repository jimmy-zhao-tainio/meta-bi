#nullable enable

namespace MetaTransformScript
{
    public sealed class SetClauseAssignmentsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public SetAssignment SetAssignment { get; set; } = null!;

        public SetClause SetClause { get; set; } = null!;

    }
}
