#nullable enable

namespace MetaDataQuality
{
    public sealed class MissingCommonFilter
    {
        public string Id { get; set; } = string.Empty;

        public string BaseObjectName { get; set; } = string.Empty;

        public string CommonPredicateDisplay { get; set; } = string.Empty;

        public string CommonPredicateSignature { get; set; } = string.Empty;

        public DataQualityCandidate DataQualityCandidate { get; set; } = null!;

        public CorpusRelationshipPattern DominantPattern { get; set; } = null!;

        public CorpusRelationshipPattern OutlierPattern { get; set; } = null!;

    }
}
