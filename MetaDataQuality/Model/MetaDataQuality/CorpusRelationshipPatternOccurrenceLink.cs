#nullable enable

namespace MetaDataQuality
{
    public sealed class CorpusRelationshipPatternOccurrenceLink
    {
        public string Id { get; set; } = string.Empty;

        public CorpusRelationshipPattern CorpusRelationshipPattern { get; set; } = null!;

        public JoinPatternOccurrence JoinPatternOccurrence { get; set; } = null!;

    }
}
