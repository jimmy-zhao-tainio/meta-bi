#nullable enable

namespace MetaTabular
{
    public sealed class TabularColumn
    {
        public string Id { get; set; } = string.Empty;

        public string? DataCategory { get; set; }

        public string DataTypeId { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Expression { get; set; }

        public string? FormatString { get; set; }

        public string? IsHidden { get; set; }

        public string? IsKey { get; set; }

        public string? IsNullable { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Ordinal { get; set; } = string.Empty;

        public string? SourceName { get; set; }

        public string? SummarizeBy { get; set; }

        public TabularTable TabularTable { get; set; } = null!;

    }
}
