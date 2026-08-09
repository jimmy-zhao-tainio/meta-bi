#nullable enable

namespace MetaTransformScript
{
    public sealed class SchemaObjectFunctionTableReferenceParametersItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public SchemaObjectFunctionTableReference SchemaObjectFunctionTableReference { get; set; } = null!;

    }
}
