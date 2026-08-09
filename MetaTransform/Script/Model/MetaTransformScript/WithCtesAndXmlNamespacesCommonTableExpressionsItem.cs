#nullable enable

namespace MetaTransformScript
{
    public sealed class WithCtesAndXmlNamespacesCommonTableExpressionsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public CommonTableExpression CommonTableExpression { get; set; } = null!;

        public WithCtesAndXmlNamespaces WithCtesAndXmlNamespaces { get; set; } = null!;

    }
}
