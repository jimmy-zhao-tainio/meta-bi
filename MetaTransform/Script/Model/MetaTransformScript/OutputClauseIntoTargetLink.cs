#nullable enable

namespace MetaTransformScript
{
    public sealed class OutputClauseIntoTargetLink
    {
        public string Id { get; set; } = string.Empty;

        public OutputClause OutputClause { get; set; } = null!;

        public SchemaObjectName SchemaObjectName { get; set; } = null!;

    }
}
