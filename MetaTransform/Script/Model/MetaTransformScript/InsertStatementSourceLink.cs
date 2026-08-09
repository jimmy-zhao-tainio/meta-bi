#nullable enable

namespace MetaTransformScript
{
    public sealed class InsertStatementSourceLink
    {
        public string Id { get; set; } = string.Empty;

        public InsertSource InsertSource { get; set; } = null!;

        public InsertStatement InsertStatement { get; set; } = null!;

    }
}
