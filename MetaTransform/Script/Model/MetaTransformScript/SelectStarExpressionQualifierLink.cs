#nullable enable

namespace MetaTransformScript
{
    public sealed class SelectStarExpressionQualifierLink
    {
        public string Id { get; set; } = string.Empty;

        public MultiPartIdentifier MultiPartIdentifier { get; set; } = null!;

        public SelectStarExpression SelectStarExpression { get; set; } = null!;

    }
}
