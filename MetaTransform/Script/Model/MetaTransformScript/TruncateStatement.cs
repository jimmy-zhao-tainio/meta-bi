#nullable enable

namespace MetaTransformScript
{
    public sealed class TruncateStatement
    {
        public string Id { get; set; } = string.Empty;

        public TSqlStatement TSqlStatement { get; set; } = null!;

    }
}
