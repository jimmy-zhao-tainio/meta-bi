#nullable enable

namespace MetaDataQuality
{
    public sealed class MinorityColumnEquivalence
    {
        public string Id { get; set; } = string.Empty;

        public DataQualityCandidate DataQualityCandidate { get; set; } = null!;

        public CorpusColumnEquivalence DominantEquivalence { get; set; } = null!;

        public CorpusColumnEquivalence OutlierEquivalence { get; set; } = null!;

    }
}
