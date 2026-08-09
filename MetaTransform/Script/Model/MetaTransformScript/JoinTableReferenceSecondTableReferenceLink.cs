#nullable enable

namespace MetaTransformScript
{
    public sealed class JoinTableReferenceSecondTableReferenceLink
    {
        public string Id { get; set; } = string.Empty;

        public JoinTableReference JoinTableReference { get; set; } = null!;

        public TableReference TableReference { get; set; } = null!;

    }
}
