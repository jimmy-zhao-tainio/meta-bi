#nullable enable

namespace MetaSql
{
    public sealed class Function
    {
        public string Id { get; set; } = string.Empty;

        public string DefinitionSql { get; set; } = string.Empty;

        public string? DeployOrdinal { get; set; }

        public string FunctionKind { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Schema Schema { get; set; } = null!;

    }
}
