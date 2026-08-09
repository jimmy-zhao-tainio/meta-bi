#nullable enable

namespace MetaDataQuality
{
    public sealed class OuterJoinNullExpansion
    {
        public string Id { get; set; } = string.Empty;

        public string OuterJoinCount { get; set; } = string.Empty;

        public DataQualityCandidate DataQualityCandidate { get; set; } = null!;

    }
}
