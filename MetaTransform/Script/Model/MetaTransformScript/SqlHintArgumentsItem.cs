#nullable enable

namespace MetaTransformScript
{
    public sealed class SqlHintArgumentsItem
    {
        public string Id { get; set; } = string.Empty;

        public string? Ordinal { get; set; }

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public SqlHint SqlHint { get; set; } = null!;

    }
}
