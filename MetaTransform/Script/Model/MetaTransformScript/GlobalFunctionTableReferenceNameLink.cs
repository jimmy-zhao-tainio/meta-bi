#nullable enable

namespace MetaTransformScript
{
    public sealed class GlobalFunctionTableReferenceNameLink
    {
        public string Id { get; set; } = string.Empty;

        public GlobalFunctionTableReference GlobalFunctionTableReference { get; set; } = null!;

        public Identifier Identifier { get; set; } = null!;

    }
}
