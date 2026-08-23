using MetaDataQuality;

namespace MetaDataQuality.Core;

public sealed class MetaDataQualityCorpusInferenceService
{
    public void Apply(
        MetaDataQualityModel model,
        CorpusInferenceOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(model);
        var effectiveOptions = options ?? CorpusInferenceOptions.Conservative;

        model.CorpusRelationshipList.Clear();
        model.CorpusRelationshipPatternList.Clear();
        model.CorpusRelationshipPatternOccurrenceLinkList.Clear();
        model.CorpusColumnEquivalenceList.Clear();
        model.CorpusColumnEquivalenceOccurrenceLinkList.Clear();
        model.DataQualityCandidateEvidenceList.Clear();
        model.MinorityJoinPatternList.Clear();
        model.IncompleteCompositeJoinList.Clear();
        model.SuspiciousExtraJoinPredicateList.Clear();
        model.MissingCommonFilterList.Clear();
        model.MinorityColumnEquivalenceList.Clear();
        model.InnerJoinAgainstUsuallyOptionalRelationshipList.Clear();
        model.LeftJoinAgainstUsuallyMandatoryRelationshipList.Clear();
        model.ImpliedForeignKeyMissingReferenceList.Clear();
        model.ImpliedUniqueKeyViolationList.Clear();
        model.ImpliedJoinFanoutRiskList.Clear();
        model.ImpliedOutputDuplicateRiskList.Clear();

        var joinPatternById = model.JoinPatternList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var keyPartsByPatternId = model.JoinPatternKeyPartList
            .GroupBy(item => item.JoinPattern.Id, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(row => CorpusInferenceNormalization.ParseOrdinalOrMax(row.Ordinal)).ToArray(),
                StringComparer.Ordinal);
        var baseTablesByOccurrenceId = model.JoinPatternOccurrenceBaseTableList
            .GroupBy(item => item.JoinPatternOccurrence.Id, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);

        var relationshipBySignature = CorpusObservationBuilder.BuildRelationshipAggregates(
            model.JoinPatternOccurrenceList,
            joinPatternById,
            keyPartsByPatternId,
            baseTablesByOccurrenceId);
        if (relationshipBySignature.Count == 0)
        {
            return;
        }

        var usedIds = new HashSet<string>(
            model.DataQualityCandidateList
                .Select(static row => row.Id)
                .Concat(model.DataQualityCandidateJoinPatternLinkList.Select(static row => row.Id))
                .Concat(model.CorpusRelationshipList.Select(static row => row.Id))
                .Concat(model.CorpusRelationshipPatternList.Select(static row => row.Id))
                .Concat(model.CorpusRelationshipPatternOccurrenceLinkList.Select(static row => row.Id))
                .Concat(model.CorpusColumnEquivalenceList.Select(static row => row.Id))
                .Concat(model.CorpusColumnEquivalenceOccurrenceLinkList.Select(static row => row.Id))
                .Concat(model.DataQualityCandidateEvidenceList.Select(static row => row.Id))
                .Concat(model.MinorityJoinPatternList.Select(static row => row.Id))
                .Concat(model.IncompleteCompositeJoinList.Select(static row => row.Id))
                .Concat(model.SuspiciousExtraJoinPredicateList.Select(static row => row.Id))
                .Concat(model.MissingCommonFilterList.Select(static row => row.Id))
                .Concat(model.MinorityColumnEquivalenceList.Select(static row => row.Id))
                .Concat(model.InnerJoinAgainstUsuallyOptionalRelationshipList.Select(static row => row.Id))
                .Concat(model.LeftJoinAgainstUsuallyMandatoryRelationshipList.Select(static row => row.Id))
                .Concat(model.ImpliedForeignKeyMissingReferenceList.Select(static row => row.Id))
                .Concat(model.ImpliedUniqueKeyViolationList.Select(static row => row.Id))
                .Concat(model.ImpliedJoinFanoutRiskList.Select(static row => row.Id))
                .Concat(model.ImpliedOutputDuplicateRiskList.Select(static row => row.Id)),
            StringComparer.Ordinal);
        var countersByPrefix = new Dictionary<string, int>(StringComparer.Ordinal);

        var materializedRelationships = CorpusPatternMaterializer.Materialize(
            model,
            relationshipBySignature,
            usedIds,
            countersByPrefix);
        var columnEquivalenceBySignature = CorpusColumnEquivalenceBuilder.BuildColumnEquivalenceAggregates(relationshipBySignature);
        var materializedColumnEquivalences = CorpusColumnEquivalenceMaterializer.Materialize(
            model,
            columnEquivalenceBySignature,
            usedIds,
            countersByPrefix);

        CorpusCandidateEmitter.Emit(
            model,
            materializedRelationships,
            materializedColumnEquivalences,
            effectiveOptions,
            usedIds,
            countersByPrefix);
    }
}
