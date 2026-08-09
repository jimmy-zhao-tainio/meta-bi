#nullable enable

namespace MetaDataQuality
{
    public sealed class LeftJoinAgainstUsuallyMandatoryRelationship
    {
        public string Id { get; set; } = string.Empty;

        public CorpusRelationshipPattern CorpusRelationshipPattern { get; set; } = null!;

        public DataQualityCandidate DataQualityCandidate { get; set; } = null!;

    }
}
