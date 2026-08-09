#nullable enable

namespace MetaTransformScript
{
    public sealed class SelectScalarExpression
    {
        public string Id { get; set; } = string.Empty;

        public SelectElement SelectElement { get; set; } = null!;

    }
}
