#nullable enable

namespace MetaDataQuality
{
    public sealed class CorpusColumnEquivalenceOccurrenceLink
    {
        public string Id { get; set; } = string.Empty;

        public CorpusColumnEquivalence CorpusColumnEquivalence { get; set; } = null!;

        public JoinPatternOccurrence JoinPatternOccurrence { get; set; } = null!;

    }
}
