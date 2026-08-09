#nullable enable

namespace MetaTransformScript
{
    public sealed class TransformScriptStatementLink
    {
        public string Id { get; set; } = string.Empty;

        public TransformScript TransformScript { get; set; } = null!;

        public TSqlStatement TSqlStatement { get; set; } = null!;

    }
}
