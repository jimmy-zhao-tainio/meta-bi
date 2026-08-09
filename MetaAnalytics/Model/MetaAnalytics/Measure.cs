#nullable enable

namespace MetaAnalytics
{
    public sealed class Measure
    {
        public string Id { get; set; } = string.Empty;

        public string? DataTypeId { get; set; }

        public string? Description { get; set; }

        public string? DisplayFolder { get; set; }

        public string? FormatString { get; set; }

        public string? IsHidden { get; set; }

        public string Name { get; set; } = string.Empty;

        public Attribute SourceAttribute { get; set; } = null!;

        public Table Table { get; set; } = null!;

    }
}
