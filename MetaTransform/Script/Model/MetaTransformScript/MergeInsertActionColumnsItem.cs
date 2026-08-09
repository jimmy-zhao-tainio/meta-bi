#nullable enable

namespace MetaTransformScript
{
    public sealed class MergeInsertActionColumnsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public Identifier Identifier { get; set; } = null!;

        public MergeInsertAction MergeInsertAction { get; set; } = null!;

    }
}
