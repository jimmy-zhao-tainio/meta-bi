#nullable enable

namespace MetaTransformScript
{
    public sealed class GlobalFunctionTableReference
    {
        public string Id { get; set; } = string.Empty;

        public TableReferenceWithAlias TableReferenceWithAlias { get; set; } = null!;

    }
}
