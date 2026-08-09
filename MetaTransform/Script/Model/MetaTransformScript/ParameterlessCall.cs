#nullable enable

namespace MetaTransformScript
{
    public sealed class ParameterlessCall
    {
        public string Id { get; set; } = string.Empty;

        public string? ParameterlessCallType { get; set; }

        public PrimaryExpression PrimaryExpression { get; set; } = null!;

    }
}
