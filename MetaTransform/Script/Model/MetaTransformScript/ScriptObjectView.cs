#nullable enable

namespace MetaTransformScript
{
    public sealed class ScriptObjectView
    {
        public string Id { get; set; } = string.Empty;

        public string TargetSqlIdentifier { get; set; } = string.Empty;

        public TransformScript TransformScript { get; set; } = null!;

    }
}
