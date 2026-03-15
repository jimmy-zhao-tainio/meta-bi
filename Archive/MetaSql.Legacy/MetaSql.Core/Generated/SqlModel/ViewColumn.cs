namespace SqlModel
{
    public sealed class ViewColumn
    {
        public string Id { get; internal set; } = string.Empty;
        public string IsNullable { get; internal set; } = string.Empty;
        public string Length { get; internal set; } = string.Empty;
        public string Name { get; internal set; } = string.Empty;
        public string Ordinal { get; internal set; } = string.Empty;
        public string Precision { get; internal set; } = string.Empty;
        public string Scale { get; internal set; } = string.Empty;
        public string TypeName { get; internal set; } = string.Empty;
        public string ViewId { get; internal set; } = string.Empty;
        public View View { get; internal set; } = new View();
    }
}
