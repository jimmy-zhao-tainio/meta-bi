#nullable enable

namespace MetaDataQuality
{
    public sealed class CorpusRelationship
    {
        public string Id { get; set; } = string.Empty;

        public string CanonicalSideAObjectName { get; set; } = string.Empty;

        public string CanonicalSideBObjectName { get; set; } = string.Empty;

        public string CanonicalUndirectedSignature { get; set; } = string.Empty;

        public string OccurrenceCount { get; set; } = string.Empty;

        public string TransformCount { get; set; } = string.Empty;

    }
}
