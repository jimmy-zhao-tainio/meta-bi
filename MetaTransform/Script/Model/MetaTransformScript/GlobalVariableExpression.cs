#nullable enable

namespace MetaTransformScript
{
    public sealed class GlobalVariableExpression
    {
        public string Id { get; set; } = string.Empty;

        public string? Name { get; set; }

        public ValueExpression ValueExpression { get; set; } = null!;

    }
}
