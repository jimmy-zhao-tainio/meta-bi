#nullable enable

namespace MetaTransformScript
{
    public sealed class SetAssignmentTargetLink
    {
        public string Id { get; set; } = string.Empty;

        public ColumnReferenceExpression ColumnReferenceExpression { get; set; } = null!;

        public SetAssignment SetAssignment { get; set; } = null!;

    }
}
