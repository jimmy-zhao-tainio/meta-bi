#nullable enable

namespace MetaTransformScript
{
    public sealed class JoinTableReference
    {
        public string Id { get; set; } = string.Empty;

        public TableReference TableReference { get; set; } = null!;

    }
}
