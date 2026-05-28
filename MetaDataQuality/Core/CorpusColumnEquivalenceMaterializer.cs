using System.Globalization;
using MetaDataQuality;

namespace MetaDataQuality.Core;

internal static class CorpusColumnEquivalenceMaterializer
{
    public static List<MaterializedColumnEquivalence> Materialize(
        MetaDataQualityModel model,
        IReadOnlyDictionary<string, ColumnEquivalenceAggregate> equivalenceBySignature,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        var occurrenceById = model.JoinPatternOccurrenceList
            .ToDictionary(static row => row.Id, StringComparer.Ordinal);
        var materialized = new List<MaterializedColumnEquivalence>();
        foreach (var aggregate in equivalenceBySignature.Values
                     .OrderBy(item => item.CanonicalUndirectedSignature, StringComparer.Ordinal))
        {
            var equivalenceId = CorpusInferenceIdAllocator.BuildUniqueId(
                usedIds,
                countersByPrefix,
                "CorpusColumnEquivalence");
            var row = new CorpusColumnEquivalence
            {
                Id = equivalenceId,
                CanonicalUndirectedSignature = aggregate.CanonicalUndirectedSignature,
                CanonicalSideAColumnName = aggregate.CanonicalSideAColumnName,
                CanonicalSideBColumnName = aggregate.CanonicalSideBColumnName,
                OccurrenceCount = aggregate.OccurrenceIds.Count.ToString(CultureInfo.InvariantCulture),
                TransformCount = aggregate.TransformScriptIds.Count.ToString(CultureInfo.InvariantCulture),
            };
            model.CorpusColumnEquivalenceList.Add(row);

            foreach (var observation in aggregate.Observations
                         .GroupBy(static item => item.OccurrenceId, StringComparer.Ordinal)
                         .Select(static group => group.First())
                         .OrderBy(static item => item.TransformScriptName, StringComparer.OrdinalIgnoreCase)
                         .ThenBy(static item => item.OccurrenceId, StringComparer.Ordinal))
            {
                if (!occurrenceById.TryGetValue(observation.OccurrenceId, out var occurrence))
                {
                    throw new InvalidOperationException($"Join pattern occurrence '{observation.OccurrenceId}' was not found while materializing corpus column equivalences.");
                }

                var linkId = CorpusInferenceIdAllocator.BuildUniqueId(
                    usedIds,
                    countersByPrefix,
                    "CorpusColumnEquivalenceOccurrenceLink");
                model.CorpusColumnEquivalenceOccurrenceLinkList.Add(new CorpusColumnEquivalenceOccurrenceLink
                {
                    Id = linkId,
                    CorpusColumnEquivalence = row,
                    JoinPatternOccurrence = occurrence,
                });
            }

            materialized.Add(new MaterializedColumnEquivalence(row, aggregate));
        }

        return materialized;
    }
}
