#nullable enable

namespace MetaAnalytics
{
    public sealed class RoleFilter
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Expression { get; set; } = string.Empty;

        public string ExpressionLanguage { get; set; } = string.Empty;

        public SecurityRole SecurityRole { get; set; } = null!;

        public Table Table { get; set; } = null!;

    }
}
