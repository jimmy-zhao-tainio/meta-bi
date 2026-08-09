#nullable enable

namespace MetaMultiDimensional
{
    public sealed class AttributeRelationship
    {
        public string Id { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? RelationshipType { get; set; }

        public DimensionAttribute ChildAttribute { get; set; } = null!;

        public DimensionAttribute ParentAttribute { get; set; } = null!;

    }
}
