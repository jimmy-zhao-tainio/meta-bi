#nullable enable

namespace MetaDataWarehouse
{
    public sealed class AggregateFact
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public Fact AggregatedFact { get; set; } = null!;

        public Fact SourceFact { get; set; } = null!;

    }
}
