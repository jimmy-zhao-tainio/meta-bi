#nullable enable

namespace MetaTransformScript
{
    public sealed class SearchedCaseExpression
    {
        public string Id { get; set; } = string.Empty;

        public CaseExpression CaseExpression { get; set; } = null!;

    }
}
