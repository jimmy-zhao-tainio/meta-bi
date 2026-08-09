#nullable enable

namespace MetaTransformScript
{
    public sealed class ScriptObjectScalarFunction
    {
        public string Id { get; set; } = string.Empty;

        public DataTypeReference DataTypeReference { get; set; } = null!;

        public ScalarExpression ScalarExpression { get; set; } = null!;

        public TransformScript TransformScript { get; set; } = null!;

    }
}
