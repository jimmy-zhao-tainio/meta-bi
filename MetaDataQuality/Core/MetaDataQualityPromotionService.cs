using MetaDataQuality;

namespace MetaDataQuality.Core;

public sealed class MetaDataQualityPromotionService
{
    public MetaDataQualityPromotionResult Promote(
        MetaDataQualityModel model,
        IReadOnlyList<string> candidateIds,
        bool promoteAll)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(candidateIds);

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
            var byId = model.DataQualityCandidateList
                .ToDictionary(item => item.Id, StringComparer.Ordinal);
            foreach (var candidateId in candidateIds.Distinct(StringComparer.Ordinal))
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
