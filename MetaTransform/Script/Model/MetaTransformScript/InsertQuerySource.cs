#nullable enable

namespace MetaTransformScript
{
    public sealed class InsertQuerySource
    {
        public string Id { get; set; } = string.Empty;

        public InsertSource InsertSource { get; set; } = null!;

    }
}
