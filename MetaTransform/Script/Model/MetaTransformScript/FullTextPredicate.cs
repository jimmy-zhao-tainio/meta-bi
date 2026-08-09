#nullable enable

namespace MetaTransformScript
{
    public sealed class FullTextPredicate
    {
        public string Id { get; set; } = string.Empty;

        public string? FullTextFunctionType { get; set; }

        public BooleanExpression BooleanExpression { get; set; } = null!;

    }
}
