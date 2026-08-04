using MetaDataQuality;

namespace MetaDataQuality.Core;

public sealed class MetaDataQualityPromotionService
{
    public MetaDataQualityPromotionResult Promote(
        MetaDataQualityModel model,
        IReadOnlyList<string> candidateIds,
        bool promoteAll,
        IReadOnlyList<string> candidateKinds)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(candidateIds);
        ArgumentNullException.ThrowIfNull(candidateKinds);

        var promotedCount = 0;
        if (promoteAll)
        {
            foreach (var candidate in model.DataQualityCandidateList)
            {
                if (!string.Equals(candidate.Status, CandidateStatuses.Promoted, StringComparison.OrdinalIgnoreCase))
                {
                    candidate.Status = CandidateStatuses.Promoted;
                    promotedCount++;
                }
            }
        }
        else
        {
            var selectedCandidateIds = candidateIds
                .Concat(ResolveCandidateIdsByKind(model, candidateKinds))
                .Where(static item => !string.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            var byId = model.DataQualityCandidateList
                .ToDictionary(item => item.Id, StringComparer.Ordinal);
            foreach (var candidateId in selectedCandidateIds)
            {
                if (!byId.TryGetValue(candidateId, out var candidate) || candidate is null)
                {
                    throw new MetaDataQualityCandidateNotFoundException(candidateId);
                }

                if (!string.Equals(candidate.Status, CandidateStatuses.Promoted, StringComparison.OrdinalIgnoreCase))
                {
                    candidate.Status = CandidateStatuses.Promoted;
                    promotedCount++;
                }
            }
        }

        var totalPromoted = model.DataQualityCandidateList.Count(item =>
            string.Equals(item.Status, CandidateStatuses.Promoted, StringComparison.OrdinalIgnoreCase));
        return new MetaDataQualityPromotionResult(promotedCount, totalPromoted);
    }

    private static IEnumerable<string> ResolveCandidateIdsByKind(
        MetaDataQualityModel model,
        IReadOnlyList<string> candidateKinds)
    {
        var requestedKinds = candidateKinds
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        foreach (var candidateKind in requestedKinds)
        {
            if (!MetaDataQualityCandidateKindMap.KnownKinds.Contains(candidateKind))
            {
                throw new MetaDataQualityCandidateKindNotFoundException(candidateKind);
            }
        }

        if (requestedKinds.Length == 0)
        {
            return [];
        }

        var requestedKindSet = requestedKinds.ToHashSet(StringComparer.Ordinal);
        return MetaDataQualityCandidateKindMap.Resolve(model)
            .Where(pair => requestedKindSet.Contains(pair.Value))
            .Select(static pair => pair.Key);
    }
}

public sealed record MetaDataQualityPromotionResult(
    int PromotedThisRunCount,
    int TotalPromotedCount);

public sealed class MetaDataQualityCandidateNotFoundException : InvalidOperationException
{
    public MetaDataQualityCandidateNotFoundException(string candidateId)
        : base($"Data quality candidate id '{candidateId}' was not found.")
    {
        CandidateId = candidateId;
    }

    public string CandidateId { get; }
}

public sealed class MetaDataQualityCandidateKindNotFoundException : InvalidOperationException
{
    public MetaDataQualityCandidateKindNotFoundException(string candidateKind)
        : base($"Data quality candidate kind '{candidateKind}' was not recognized.")
    {
        CandidateKind = candidateKind;
    }

    public string CandidateKind { get; }
}
