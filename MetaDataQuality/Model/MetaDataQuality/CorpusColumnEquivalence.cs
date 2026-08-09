#nullable enable

namespace MetaDataQuality
{
    public sealed class CorpusColumnEquivalence
    {
        public string Id { get; set; } = string.Empty;

        public string CanonicalSideAColumnName { get; set; } = string.Empty;

        public string CanonicalSideBColumnName { get; set; } = string.Empty;

        public string CanonicalUndirectedSignature { get; set; } = string.Empty;

        public string OccurrenceCount { get; set; } = string.Empty;

        public string TransformCount { get; set; } = string.Empty;

    }
}
