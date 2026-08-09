#nullable enable

namespace MetaTransformScript
{
    public sealed class SearchedWhenClause
    {
        public string Id { get; set; } = string.Empty;

        public WhenClause WhenClause { get; set; } = null!;

    }
}
