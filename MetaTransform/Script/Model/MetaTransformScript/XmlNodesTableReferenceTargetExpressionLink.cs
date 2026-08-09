#nullable enable

namespace MetaTransformScript
{
    public sealed class XmlNodesTableReferenceTargetExpressionLink
    {
        public string Id { get; set; } = string.Empty;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public XmlNodesTableReference XmlNodesTableReference { get; set; } = null!;

    }
}
