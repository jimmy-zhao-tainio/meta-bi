#nullable enable

namespace MetaTransformScript
{
    public sealed class UpdateStatement
    {
        public string Id { get; set; } = string.Empty;

        public StatementWithCtesAndXmlNamespaces StatementWithCtesAndXmlNamespaces { get; set; } = null!;

    }
}
