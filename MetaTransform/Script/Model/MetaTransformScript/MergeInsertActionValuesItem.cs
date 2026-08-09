#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeInsertActionValuesItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public MergeInsertAction MergeInsertAction { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
