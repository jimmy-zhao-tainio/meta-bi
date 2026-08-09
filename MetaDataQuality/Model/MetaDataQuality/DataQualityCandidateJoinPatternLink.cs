#nullable enable

namespace MetaDataQuality
{
    public sealed class DataQualityCandidateJoinPatternLink
    {
        public string Id { get; set; } = string.Empty;

        public DataQualityCandidate DataQualityCandidate { get; set; } = null!;

        public JoinPattern JoinPattern { get; set; } = null!;

    }
}
