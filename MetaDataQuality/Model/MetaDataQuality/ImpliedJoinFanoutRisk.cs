#nullable enable

namespace MetaDataQuality
{
    public sealed class ImpliedJoinFanoutRisk
    {
        public string Id { get; set; } = string.Empty;

        public DataQualityCandidate DataQualityCandidate { get; set; } = null!;

        public CorpusRelationshipPattern DominantPattern { get; set; } = null!;

    }
}
