#nullable enable

namespace MetaTransformScript
{
    public sealed class StoredProcedureContract
    {
        public string Id { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public ScriptObjectStoredProcedure ScriptObjectStoredProcedure { get; set; } = null!;

    }
}
