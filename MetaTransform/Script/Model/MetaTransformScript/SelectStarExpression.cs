#nullable enable

namespace MetaTransformScript
{
    public sealed class SelectStarExpression
    {
        public string Id { get; set; } = string.Empty;

        public SelectElement SelectElement { get; set; } = null!;

    }
}
