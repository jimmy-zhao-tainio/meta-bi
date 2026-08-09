#nullable enable

namespace MetaSql
{
    public sealed class View
    {
        public string Id { get; set; } = string.Empty;

        public string DefinitionSql { get; set; } = string.Empty;

        public string? DeployOrdinal { get; set; }

        public string Name { get; set; } = string.Empty;

        public Schema Schema { get; set; } = null!;

    }
}
