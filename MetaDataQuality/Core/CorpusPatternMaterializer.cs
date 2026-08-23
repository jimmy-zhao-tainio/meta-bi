using System.Globalization;
using MetaDataQuality;

namespace MetaDataQuality.Core;

internal static class CorpusPatternMaterializer
{
    public static List<MaterializedRelationship> Materialize(
        MetaDataQualityModel model,
        IReadOnlyDictionary<string, RelationshipAggregate> relationshipBySignature,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        var occurrenceById = model.JoinPatternOccurrenceList
            .ToDictionary(static row => row.Id, StringComparer.Ordinal);
        var materializedRelationships = new List<MaterializedRelationship>();
        foreach (var relationship in relationshipBySignature.Values
                     .OrderBy(item => item.RelationshipSignature, StringComparer.Ordinal))
        {
            var relationshipId = CorpusInferenceIdAllocator.BuildUniqueId(
                usedIds,
                countersByPrefix,
                "CorpusRelationship");
            var relationshipRow = new CorpusRelationship
            {
                Id = relationshipId,
                CanonicalUndirectedSignature = relationship.RelationshipSignature,
                CanonicalSideAObjectName = relationship.CanonicalSideAObjectName,
                CanonicalSideBObjectName = relationship.CanonicalSideBObjectName,
                OccurrenceCount = relationship.OccurrenceIds.Count.ToString(CultureInfo.InvariantCulture),
                TransformCount = relationship.TransformScriptIds.Count.ToString(CultureInfo.InvariantCulture),
            };
            model.CorpusRelationshipList.Add(relationshipRow);

            var patternRows = new List<MaterializedPattern>();
            foreach (var pattern in relationship.Patterns.Values
                         .OrderBy(item => item.CanonicalPatternSignature, StringComparer.Ordinal))
            {
                var patternId = CorpusInferenceIdAllocator.BuildUniqueId(
                    usedIds,
                    countersByPrefix,
                    "CorpusRelationshipPattern");
                var ratio = relationship.OccurrenceIds.Count == 0
                    ? 0d
                    : pattern.OccurrenceIds.Count / (double)relationship.OccurrenceIds.Count;
                var representative = pattern.Observations
                    .OrderBy(static item => item.TransformScriptName, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(static item => item.OccurrenceId, StringComparer.Ordinal)
                    .First();
                var patternRow = new CorpusRelationshipPattern
                {
                    Id = patternId,
                    CorpusRelationship = relationshipRow,
                    CanonicalKeyPartSetSignature = pattern.CanonicalPatternSignature,
                    RepresentativeDirectionalSignature = representative.DirectionalSignature,
                    KeyPartCount = pattern.KeyPartCount.ToString(CultureInfo.InvariantCulture),
                    OccurrenceCount = pattern.OccurrenceIds.Count.ToString(CultureInfo.InvariantCulture),
                    TransformCount = pattern.TransformScriptIds.Count.ToString(CultureInfo.InvariantCulture),
                    OccurrenceRatio = CorpusInferenceNormalization.FormatRatio(ratio),
                    IsDominant = "false",
                };
                model.CorpusRelationshipPatternList.Add(patternRow);

                foreach (var observation in pattern.Observations
                             .OrderBy(static item => item.TransformScriptName, StringComparer.OrdinalIgnoreCase)
                             .ThenBy(static item => item.OccurrenceId, StringComparer.Ordinal))
                {
                    if (!occurrenceById.TryGetValue(observation.OccurrenceId, out var occurrence))
                    {
                        throw new InvalidOperationException($"Join pattern occurrence '{observation.OccurrenceId}' was not found while materializing corpus relationship patterns.");
                    }

                    var occurrenceLinkId = CorpusInferenceIdAllocator.BuildUniqueId(
                        usedIds,
                        countersByPrefix,
                        "CorpusRelationshipPatternOccurrenceLink");
                    model.CorpusRelationshipPatternOccurrenceLinkList.Add(new CorpusRelationshipPatternOccurrenceLink
                    {
                        Id = occurrenceLinkId,
                        CorpusRelationshipPattern = patternRow,
                        JoinPatternOccurrence = occurrence,
                    });
                }

                patternRows.Add(new MaterializedPattern(patternRow, pattern));
            }

            materializedRelationships.Add(new MaterializedRelationship(relationshipRow, relationship, patternRows));
        }

        return materializedRelationships;
    }
}
