#nullable enable

namespace MetaTransformScript
{
    public sealed class OverClauseWindowNameLink
    {
        public string Id { get; set; } = string.Empty;

        public Identifier Identifier { get; set; } = null!;

        public OverClause OverClause { get; set; } = null!;

    }
}
