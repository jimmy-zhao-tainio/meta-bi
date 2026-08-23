#nullable enable
using System;
using System.Collections.Generic;

namespace MetaDataQuality;
public sealed partial class CorpusColumnEquivalence
{
    public string Id { get; set; } = null !;
    public string CanonicalSideAColumnName { get; set; } = null !;
    public string CanonicalSideBColumnName { get; set; } = null !;
    public string CanonicalUndirectedSignature { get; set; } = null !;
    public string OccurrenceCount { get; set; } = null !;
    public string TransformCount { get; set; } = null !;
}

public sealed partial class CorpusColumnEquivalenceOccurrenceLink
{
    public string Id { get; set; } = null !;
    public CorpusColumnEquivalence CorpusColumnEquivalence { get; set; } = null !;
    public JoinPatternOccurrence JoinPatternOccurrence { get; set; } = null !;
}

public sealed partial class CorpusRelationship
{
    public string Id { get; set; } = null !;
    public string CanonicalSideAObjectName { get; set; } = null !;
    public string CanonicalSideBObjectName { get; set; } = null !;
    public string CanonicalUndirectedSignature { get; set; } = null !;
    public string OccurrenceCount { get; set; } = null !;
    public string TransformCount { get; set; } = null !;
}

public sealed partial class CorpusRelationshipPattern
{
    public string Id { get; set; } = null !;
    public string CanonicalKeyPartSetSignature { get; set; } = null !;
    public string IsDominant { get; set; } = null !;
    public string KeyPartCount { get; set; } = null !;
    public string OccurrenceCount { get; set; } = null !;
    public string OccurrenceRatio { get; set; } = null !;
    public string RepresentativeDirectionalSignature { get; set; } = null !;
    public string TransformCount { get; set; } = null !;
    public CorpusRelationship CorpusRelationship { get; set; } = null !;
}

public sealed partial class CorpusRelationshipPatternOccurrenceLink
{
    public string Id { get; set; } = null !;
    public CorpusRelationshipPattern CorpusRelationshipPattern { get; set; } = null !;
    public JoinPatternOccurrence JoinPatternOccurrence { get; set; } = null !;
}

public sealed partial class DataQualityCandidate
{
    public string Id { get; set; } = null !;
    public string? Assumptions { get; set; }
    public string Name { get; set; } = null !;
    public string? Rationale { get; set; }
    public string? SqlTemplate { get; set; }
    public string Status { get; set; } = null !;
}

public sealed partial class DataQualityCandidateEvidence
{
    public string Id { get; set; } = null !;
    public string ConfidenceBand { get; set; } = null !;
    public string ConfidenceReason { get; set; } = null !;
    public string ConsensusRatio { get; set; } = null !;
    public string DistinctRelationshipPatternCount { get; set; } = null !;
    public string DistinctSourceObjectCount { get; set; } = null !;
    public string DistinctSourceTransformCount { get; set; } = null !;
    public string DistinctTransformCount { get; set; } = null !;
    public string EffectiveTransformCount { get; set; } = null !;
    public string EvidenceDiversitySummary { get; set; } = null !;
    public string EvidenceQuality { get; set; } = null !;
    public string EvidenceType { get; set; } = null !;
    public string Explanation { get; set; } = null !;
    public string OccurrenceCount { get; set; } = null !;
    public string OutlierRatio { get; set; } = null !;
    public string TransformCount { get; set; } = null !;
    public CorpusRelationship CorpusRelationship { get; set; } = null !;
    public CorpusRelationshipPattern? CorpusRelationshipPattern { get; set; }
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
}

public sealed partial class DataQualityCandidateJoinPatternLink
{
    public string Id { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
    public JoinPattern JoinPattern { get; set; } = null !;
}

public sealed partial class FilterPredicateObservation
{
    public string Id { get; set; } = null !;
    public string BaseObjectName { get; set; } = null !;
    public string PredicateDisplay { get; set; } = null !;
    public string PredicateSignature { get; set; } = null !;
    public JoinPatternOccurrence JoinPatternOccurrence { get; set; } = null !;
}

public sealed partial class ImpliedForeignKeyMissingReference
{
    public string Id { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
    public CorpusRelationshipPattern DominantPattern { get; set; } = null !;
}

public sealed partial class ImpliedJoinFanoutRisk
{
    public string Id { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
    public CorpusRelationshipPattern DominantPattern { get; set; } = null !;
}

public sealed partial class ImpliedOutputDuplicateRisk
{
    public string Id { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
    public CorpusRelationshipPattern DominantPattern { get; set; } = null !;
}

public sealed partial class ImpliedUniqueKeyViolation
{
    public string Id { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
    public CorpusRelationshipPattern DominantPattern { get; set; } = null !;
}

public sealed partial class IncompleteCompositeJoin
{
    public string Id { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
    public CorpusRelationshipPattern DominantPattern { get; set; } = null !;
    public CorpusRelationshipPattern OutlierPattern { get; set; } = null !;
}

public sealed partial class InnerJoinAgainstUsuallyOptionalRelationship
{
    public string Id { get; set; } = null !;
    public CorpusRelationshipPattern CorpusRelationshipPattern { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
}

public sealed partial class JoinMultiplicityExplosion
{
    public string Id { get; set; } = null !;
    public string EqualityPredicateCount { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
}

public sealed partial class JoinOrphan
{
    public string Id { get; set; } = null !;
    public string EqualityPredicateCount { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
}

public sealed partial class JoinPattern
{
    public string Id { get; set; } = null !;
    public string CanonicalSignature { get; set; } = null !;
    public string ContainsEqualityPredicate { get; set; } = null !;
    public string EqualityPredicateCount { get; set; } = null !;
    public string? QualifiedJoinType { get; set; }
}

public sealed partial class JoinPatternKeyPart
{
    public string Id { get; set; } = null !;
    public string BooleanComparisonExpressionId { get; set; } = null !;
    public string? FirstExpressionDisplay { get; set; }
    public string FirstExpressionId { get; set; } = null !;
    public string? FirstJoinInputColumnName { get; set; }
    public string? FirstJoinInputObjectName { get; set; }
    public string Ordinal { get; set; } = null !;
    public string? SecondExpressionDisplay { get; set; }
    public string SecondExpressionId { get; set; } = null !;
    public string? SecondJoinInputColumnName { get; set; }
    public string? SecondJoinInputObjectName { get; set; }
    public JoinPattern JoinPattern { get; set; } = null !;
}

public sealed partial class JoinPatternKeyPartInputObjectIdentifierPart
{
    public string Id { get; set; } = null !;
    public string InputSide { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public string Value { get; set; } = null !;
    public JoinPatternKeyPart JoinPatternKeyPart { get; set; } = null !;
}

public sealed partial class JoinPatternOccurrence
{
    public string Id { get; set; } = null !;
    public string? CteId { get; set; }
    public string? CteName { get; set; }
    public string? FirstTableReferenceId { get; set; }
    public string JoinTableReferenceId { get; set; } = null !;
    public string QualifiedJoinId { get; set; } = null !;
    public string QueryExpressionId { get; set; } = null !;
    public string QuerySpecificationId { get; set; } = null !;
    public string? ScopePath { get; set; }
    public string? SearchConditionBooleanExpressionId { get; set; }
    public string? SecondTableReferenceId { get; set; }
    public string TransformScriptId { get; set; } = null !;
    public string TransformScriptName { get; set; } = null !;
    public JoinPattern JoinPattern { get; set; } = null !;
}

public sealed partial class JoinPatternOccurrenceBaseTable
{
    public string Id { get; set; } = null !;
    public string BaseNamedTableReferenceId { get; set; } = null !;
    public string BaseObjectName { get; set; } = null !;
    public string BaseSchemaObjectNameId { get; set; } = null !;
    public string BaseTableReferenceId { get; set; } = null !;
    public string JoinInputTableReferenceId { get; set; } = null !;
    public string ResolutionDepth { get; set; } = null !;
    public string? ResolutionPath { get; set; }
    public string? ResolvedInCteId { get; set; }
    public string? ResolvedInCteName { get; set; }
    public JoinPatternOccurrence JoinPatternOccurrence { get; set; } = null !;
}

public sealed partial class JoinPatternOccurrenceSignal
{
    public string Id { get; set; } = null !;
    public string? Explanation { get; set; }
    public string SignalKind { get; set; } = null !;
    public string SourceCandidateKind { get; set; } = null !;
    public JoinPatternOccurrence JoinPatternOccurrence { get; set; } = null !;
}

public sealed partial class LeftJoinAgainstUsuallyMandatoryRelationship
{
    public string Id { get; set; } = null !;
    public CorpusRelationshipPattern CorpusRelationshipPattern { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
}

public sealed partial class MinorityColumnEquivalence
{
    public string Id { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
    public CorpusColumnEquivalence DominantEquivalence { get; set; } = null !;
    public CorpusColumnEquivalence OutlierEquivalence { get; set; } = null !;
}

public sealed partial class MinorityJoinPattern
{
    public string Id { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
    public CorpusRelationshipPattern DominantPattern { get; set; } = null !;
    public CorpusRelationshipPattern OutlierPattern { get; set; } = null !;
}

public sealed partial class MissingCommonFilter
{
    public string Id { get; set; } = null !;
    public string BaseObjectName { get; set; } = null !;
    public string CommonPredicateDisplay { get; set; } = null !;
    public string CommonPredicateSignature { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
    public CorpusRelationshipPattern DominantPattern { get; set; } = null !;
    public CorpusRelationshipPattern OutlierPattern { get; set; } = null !;
}

public sealed partial class OuterJoinNullExpansion
{
    public string Id { get; set; } = null !;
    public string OuterJoinCount { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
}

public sealed partial class OutputDuplicateRisk
{
    public string Id { get; set; } = null !;
    public string HasDistinct { get; set; } = null !;
    public string HasGroupBy { get; set; } = null !;
    public string QualifiedJoinCount { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
}

public sealed partial class SuspiciousExtraJoinPredicate
{
    public string Id { get; set; } = null !;
    public DataQualityCandidate DataQualityCandidate { get; set; } = null !;
    public CorpusRelationshipPattern DominantPattern { get; set; } = null !;
    public CorpusRelationshipPattern OutlierPattern { get; set; } = null !;
}

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
    public List<JoinPatternKeyPartInputObjectIdentifierPart> JoinPatternKeyPartInputObjectIdentifierPartList { get; set; } = new();
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

public static partial class MetaDataQualityInstance
{
    private static readonly MetaDataQualityModel _builtIn = CreateBuiltIn();
    public static MetaDataQualityModel BuiltIn => _builtIn;

    public static MetaDataQualityModel CreateBuiltIn()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        return model;
    }
}
