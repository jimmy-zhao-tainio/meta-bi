#nullable enable

namespace MetaTransformScript
{
    public sealed class SetAssignmentValueLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public SetAssignment SetAssignment { get; set; } = null!;

    }
}
