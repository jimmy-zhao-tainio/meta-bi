#nullable enable

namespace MetaDataQuality
{
    public sealed class JoinPatternOccurrenceSignal
    {
        public string Id { get; set; } = string.Empty;

        public string? Explanation { get; set; }

        public string SignalKind { get; set; } = string.Empty;

        public string SourceCandidateKind { get; set; } = string.Empty;

        public JoinPatternOccurrence JoinPatternOccurrence { get; set; } = null!;

    }
}
