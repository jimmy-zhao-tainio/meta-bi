#nullable enable

namespace MetaTransformScript
{
    public sealed class GlobalFunctionTableReferenceParametersItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public GlobalFunctionTableReference GlobalFunctionTableReference { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

    }
}
