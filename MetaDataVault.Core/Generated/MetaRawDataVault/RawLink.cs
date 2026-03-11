namespace MetaRawDataVault
{
    public sealed class RawLink
    {
        public string Id { get; internal set; } = string.Empty;
        public string LinkKind { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string SourceTableRelationshipId { get; internal set; } = string.Empty;
        public SourceTableRelationship SourceTableRelationship { get; internal set; } = new SourceTableRelationship();
    }
}
