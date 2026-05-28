namespace MetaDataQuality.Core;

internal static class CorpusInferenceIdAllocator
{
    public static string BuildUniqueId(
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix,
        string prefix)
    {
        countersByPrefix.TryGetValue(prefix, out var counter);
        while (true)
        {
            counter++;
            var candidate = $"{prefix}.{counter}";
            if (usedIds.Add(candidate))
            {
                countersByPrefix[prefix] = counter;
                return candidate;
            }
        }
    }
}
