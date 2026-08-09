#nullable enable

namespace MetaTransformScript
{
    public sealed class FunctionCallOverClauseLink
    {
        public string Id { get; set; } = string.Empty;

        public FunctionCall FunctionCall { get; set; } = null!;

        public OverClause OverClause { get; set; } = null!;

    }
}
