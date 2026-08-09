#nullable enable

namespace MetaDataQuality
{
    public sealed class FilterPredicateObservation
    {
        public string Id { get; set; } = string.Empty;

        public string BaseObjectName { get; set; } = string.Empty;

        public string PredicateDisplay { get; set; } = string.Empty;

        public string PredicateSignature { get; set; } = string.Empty;

        public JoinPatternOccurrence JoinPatternOccurrence { get; set; } = null!;

    }
}
