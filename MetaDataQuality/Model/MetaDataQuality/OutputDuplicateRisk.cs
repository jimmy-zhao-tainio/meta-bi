#nullable enable

namespace MetaDataQuality
{
    public sealed class OutputDuplicateRisk
    {
        public string Id { get; set; } = string.Empty;

        public string HasDistinct { get; set; } = string.Empty;

        public string HasGroupBy { get; set; } = string.Empty;

        public string QualifiedJoinCount { get; set; } = string.Empty;

        public DataQualityCandidate DataQualityCandidate { get; set; } = null!;

    }
}
