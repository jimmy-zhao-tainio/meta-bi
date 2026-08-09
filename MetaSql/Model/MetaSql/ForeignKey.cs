#nullable enable

namespace MetaSql
{
    public sealed class ForeignKey
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Table SourceTable { get; set; } = null!;

        public Table TargetTable { get; set; } = null!;

    }
}
