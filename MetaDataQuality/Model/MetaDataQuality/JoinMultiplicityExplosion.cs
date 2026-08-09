#nullable enable

namespace MetaDataQuality
{
    public sealed class JoinMultiplicityExplosion
    {
        public string Id { get; set; } = string.Empty;

        public string EqualityPredicateCount { get; set; } = string.Empty;

        public DataQualityCandidate DataQualityCandidate { get; set; } = null!;

    }
}
