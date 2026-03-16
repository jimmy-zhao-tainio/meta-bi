namespace MetaSchema
{
    public sealed class Schema
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SystemId { get; set; } = string.Empty;
        public System System { get; set; } = new System();
    }
}
