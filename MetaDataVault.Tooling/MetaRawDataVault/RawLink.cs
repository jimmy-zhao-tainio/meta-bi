namespace MetaRawDataVault
{
    public sealed class RawLink
    {
        public string Id { get; set; } = string.Empty;
        public string LinkKind { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SourceTableRelationshipId { get; set; } = string.Empty;
        public SourceTableRelationship SourceTableRelationship { get; set; } = new SourceTableRelationship();
    }
}
