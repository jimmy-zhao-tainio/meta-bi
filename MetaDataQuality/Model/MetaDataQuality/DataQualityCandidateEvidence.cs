#nullable enable

namespace MetaDataQuality
{
    public sealed class DataQualityCandidateEvidence
    {
        public string Id { get; set; } = string.Empty;

        public string ConfidenceBand { get; set; } = string.Empty;

        public string ConfidenceReason { get; set; } = string.Empty;

        public string ConsensusRatio { get; set; } = string.Empty;

        public string DistinctRelationshipPatternCount { get; set; } = string.Empty;

        public string DistinctSourceObjectCount { get; set; } = string.Empty;

        public string DistinctSourceTransformCount { get; set; } = string.Empty;

        public string DistinctTransformCount { get; set; } = string.Empty;

        public string EffectiveTransformCount { get; set; } = string.Empty;

        public string EvidenceDiversitySummary { get; set; } = string.Empty;

        public string EvidenceQuality { get; set; } = string.Empty;

        public string EvidenceType { get; set; } = string.Empty;

        public string Explanation { get; set; } = string.Empty;

        public string OccurrenceCount { get; set; } = string.Empty;

        public string OutlierRatio { get; set; } = string.Empty;

        public string TransformCount { get; set; } = string.Empty;

        public CorpusRelationship CorpusRelationship { get; set; } = null!;

        public CorpusRelationshipPattern? CorpusRelationshipPattern { get; set; }

        public DataQualityCandidate DataQualityCandidate { get; set; } = null!;

    }
}
