#nullable enable

namespace MetaDataQuality
{
    public sealed class DataQualityCandidate
    {
        public string Id { get; set; } = string.Empty;

        public string? Assumptions { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Rationale { get; set; }

        public string? SqlTemplate { get; set; }

        public string Status { get; set; } = string.Empty;

    }
}
