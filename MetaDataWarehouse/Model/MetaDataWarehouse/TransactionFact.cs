#nullable enable

namespace MetaDataWarehouse
{
    public sealed class TransactionFact
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public Fact Fact { get; set; } = null!;

    }
}
