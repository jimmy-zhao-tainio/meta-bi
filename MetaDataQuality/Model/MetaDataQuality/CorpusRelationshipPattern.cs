#nullable enable

namespace MetaDataQuality
{
    public sealed class CorpusRelationshipPattern
    {
        public string Id { get; set; } = string.Empty;

        public string CanonicalKeyPartSetSignature { get; set; } = string.Empty;

        public string IsDominant { get; set; } = string.Empty;

        public string KeyPartCount { get; set; } = string.Empty;

        public string OccurrenceCount { get; set; } = string.Empty;

        public string OccurrenceRatio { get; set; } = string.Empty;

        public string RepresentativeDirectionalSignature { get; set; } = string.Empty;

        public string TransformCount { get; set; } = string.Empty;

        public CorpusRelationship CorpusRelationship { get; set; } = null!;

    }
}
