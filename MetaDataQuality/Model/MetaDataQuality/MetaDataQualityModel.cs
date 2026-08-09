#nullable enable

using System.Collections.Generic;

namespace MetaDataQuality
{
    public sealed partial class MetaDataQualityModel
    {
        public static MetaDataQualityModel CreateEmpty() => new();

        public List<CorpusColumnEquivalence> CorpusColumnEquivalenceList { get; set; } = new();
        public List<CorpusColumnEquivalenceOccurrenceLink> CorpusColumnEquivalenceOccurrenceLinkList { get; set; } = new();
        public List<CorpusRelationship> CorpusRelationshipList { get; set; } = new();
        public List<CorpusRelationshipPattern> CorpusRelationshipPatternList { get; set; } = new();
        public List<CorpusRelationshipPatternOccurrenceLink> CorpusRelationshipPatternOccurrenceLinkList { get; set; } = new();
        public List<DataQualityCandidate> DataQualityCandidateList { get; set; } = new();
        public List<DataQualityCandidateEvidence> DataQualityCandidateEvidenceList { get; set; } = new();
        public List<DataQualityCandidateJoinPatternLink> DataQualityCandidateJoinPatternLinkList { get; set; } = new();
        public List<FilterPredicateObservation> FilterPredicateObservationList { get; set; } = new();
        public List<ImpliedForeignKeyMissingReference> ImpliedForeignKeyMissingReferenceList { get; set; } = new();
        public List<ImpliedJoinFanoutRisk> ImpliedJoinFanoutRiskList { get; set; } = new();
        public List<ImpliedOutputDuplicateRisk> ImpliedOutputDuplicateRiskList { get; set; } = new();
        public List<ImpliedUniqueKeyViolation> ImpliedUniqueKeyViolationList { get; set; } = new();
        public List<IncompleteCompositeJoin> IncompleteCompositeJoinList { get; set; } = new();
        public List<InnerJoinAgainstUsuallyOptionalRelationship> InnerJoinAgainstUsuallyOptionalRelationshipList { get; set; } = new();
        public List<JoinMultiplicityExplosion> JoinMultiplicityExplosionList { get; set; } = new();
        public List<JoinOrphan> JoinOrphanList { get; set; } = new();
        public List<JoinPattern> JoinPatternList { get; set; } = new();
        public List<JoinPatternKeyPart> JoinPatternKeyPartList { get; set; } = new();
        public List<JoinPatternOccurrence> JoinPatternOccurrenceList { get; set; } = new();
        public List<JoinPatternOccurrenceBaseTable> JoinPatternOccurrenceBaseTableList { get; set; } = new();
        public List<JoinPatternOccurrenceSignal> JoinPatternOccurrenceSignalList { get; set; } = new();
        public List<LeftJoinAgainstUsuallyMandatoryRelationship> LeftJoinAgainstUsuallyMandatoryRelationshipList { get; set; } = new();
        public List<MinorityColumnEquivalence> MinorityColumnEquivalenceList { get; set; } = new();
        public List<MinorityJoinPattern> MinorityJoinPatternList { get; set; } = new();
        public List<MissingCommonFilter> MissingCommonFilterList { get; set; } = new();
        public List<OuterJoinNullExpansion> OuterJoinNullExpansionList { get; set; } = new();
        public List<OutputDuplicateRisk> OutputDuplicateRiskList { get; set; } = new();
        public List<SuspiciousExtraJoinPredicate> SuspiciousExtraJoinPredicateList { get; set; } = new();
    }
}
