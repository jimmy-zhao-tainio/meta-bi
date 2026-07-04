using MetaDataQuality;

namespace MetaDataQuality.Core;

internal static class MetaDataQualityCandidateKindMap
{
    public static readonly IReadOnlySet<string> KnownKinds = new HashSet<string>(StringComparer.Ordinal)
    {
        CandidateKinds.JoinOrphan,
        CandidateKinds.OuterJoinNullExpansion,
        CandidateKinds.JoinMultiplicityExplosion,
        CandidateKinds.OutputDuplicateRisk,
        CandidateKinds.MinorityJoinPattern,
        CandidateKinds.IncompleteCompositeJoin,
        CandidateKinds.SuspiciousExtraJoinPredicate,
        CandidateKinds.MissingCommonFilter,
        CandidateKinds.MinorityColumnEquivalence,
        CandidateKinds.InnerJoinAgainstUsuallyOptionalRelationship,
        CandidateKinds.LeftJoinAgainstUsuallyMandatoryRelationship,
        CandidateKinds.ImpliedForeignKeyMissingReference,
        CandidateKinds.ImpliedUniqueKeyViolation,
        CandidateKinds.ImpliedJoinFanoutRisk,
        CandidateKinds.ImpliedOutputDuplicateRisk,
    };

    public static IReadOnlyDictionary<string, string> Resolve(MetaDataQualityModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        AddType(map, model.JoinOrphanList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.JoinOrphan);
        AddType(map, model.OuterJoinNullExpansionList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.OuterJoinNullExpansion);
        AddType(map, model.JoinMultiplicityExplosionList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.JoinMultiplicityExplosion);
        AddType(map, model.OutputDuplicateRiskList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.OutputDuplicateRisk);
        AddType(map, model.MinorityJoinPatternList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.MinorityJoinPattern);
        AddType(map, model.IncompleteCompositeJoinList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.IncompleteCompositeJoin);
        AddType(map, model.SuspiciousExtraJoinPredicateList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.SuspiciousExtraJoinPredicate);
        AddType(map, model.MissingCommonFilterList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.MissingCommonFilter);
        AddType(map, model.MinorityColumnEquivalenceList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.MinorityColumnEquivalence);
        AddType(map, model.InnerJoinAgainstUsuallyOptionalRelationshipList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.InnerJoinAgainstUsuallyOptionalRelationship);
        AddType(map, model.LeftJoinAgainstUsuallyMandatoryRelationshipList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.LeftJoinAgainstUsuallyMandatoryRelationship);
        AddType(map, model.ImpliedForeignKeyMissingReferenceList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.ImpliedForeignKeyMissingReference);
        AddType(map, model.ImpliedUniqueKeyViolationList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.ImpliedUniqueKeyViolation);
        AddType(map, model.ImpliedJoinFanoutRiskList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.ImpliedJoinFanoutRisk);
        AddType(map, model.ImpliedOutputDuplicateRiskList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.ImpliedOutputDuplicateRisk);
        return map;
    }

    private static void AddType(
        IDictionary<string, string> map,
        IEnumerable<string> candidateIds,
        string candidateType)
    {
        foreach (var candidateId in candidateIds.Where(static id => !string.IsNullOrWhiteSpace(id)))
        {
            if (!map.TryAdd(candidateId, candidateType))
            {
                map[candidateId] = candidateType;
            }
        }
    }
}
