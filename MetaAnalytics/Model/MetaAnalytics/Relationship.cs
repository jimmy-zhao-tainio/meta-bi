#nullable enable

namespace MetaAnalytics
{
    public sealed class Relationship
    {
        public string Id { get; set; } = string.Empty;

        public string Cardinality { get; set; } = string.Empty;

        public string? CrossFilterDirection { get; set; }

        public string? Description { get; set; }

        public string? IsActive { get; set; }

        public string? IsRequired { get; set; }

        public string Name { get; set; } = string.Empty;

        public string RelationshipKind { get; set; } = string.Empty;

        public string? RoleName { get; set; }

        public Attribute FromAttribute { get; set; } = null!;

        public Table FromTable { get; set; } = null!;

        public Attribute? GranularityAttribute { get; set; }

        public Table? IntermediateTable { get; set; }

        public Attribute ToAttribute { get; set; } = null!;

        public Table ToTable { get; set; } = null!;

    }
}
