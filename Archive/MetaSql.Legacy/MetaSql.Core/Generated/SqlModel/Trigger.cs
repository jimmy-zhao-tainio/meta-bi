namespace SqlModel
{
    public sealed class Trigger
    {
        public string Id { get; internal set; } = string.Empty;
        public string DefinitionSql { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string TriggerEvents { get; internal set; } = string.Empty;
        public string TriggerTiming { get; internal set; } = string.Empty;
        public string TableId { get; internal set; } = string.Empty;
        public Table Table { get; internal set; } = new Table();
    }
}
