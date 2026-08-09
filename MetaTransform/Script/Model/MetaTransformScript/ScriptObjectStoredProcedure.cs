#nullable enable

namespace MetaTransformScript
{
    public sealed class ScriptObjectStoredProcedure
    {
        public string Id { get; set; } = string.Empty;

        public string DefinitionSql { get; set; } = string.Empty;

        public TransformScript TransformScript { get; set; } = null!;

    }
}
