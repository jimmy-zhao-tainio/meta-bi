namespace MetaSchema
{
    public sealed class Schema
    {
        public string Id { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string SystemId { get; internal set; } = string.Empty;
        public System System { get; internal set; } = new System();
    }
}
