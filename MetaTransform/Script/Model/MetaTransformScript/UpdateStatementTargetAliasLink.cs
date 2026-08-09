#nullable enable

namespace MetaTransformScript
{
    public sealed class UpdateStatementTargetAliasLink
    {
        public string Id { get; set; } = string.Empty;

        public Identifier Identifier { get; set; } = null!;

        public UpdateStatement UpdateStatement { get; set; } = null!;

    }
}
