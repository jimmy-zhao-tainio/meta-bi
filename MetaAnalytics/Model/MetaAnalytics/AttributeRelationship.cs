#nullable enable

namespace MetaAnalytics
{
    public sealed class AttributeRelationship
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? RelationshipType { get; set; }

        public Attribute ChildAttribute { get; set; } = null!;

        public Attribute ParentAttribute { get; set; } = null!;

    }
}
