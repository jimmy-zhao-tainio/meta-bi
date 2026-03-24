using Meta.Core.Domain;

namespace MetaSql;

/// <summary>
/// Precomputed immutable lookup state for manifest planning.
/// </summary>
internal sealed record ManifestPlanningLookupContext
{
    public required Workspace SourceWorkspace { get; init; }
    public required Workspace LiveWorkspace { get; init; }
    public required IReadOnlyDictionary<string, GenericRecord> SourceColumnsById { get; init; }
    public required IReadOnlyDictionary<string, GenericRecord> LiveColumnsById { get; init; }
    public required IReadOnlyDictionary<string, GenericRecord> SourceTablesById { get; init; }
    public required IReadOnlyDictionary<string, GenericRecord> LiveTablesById { get; init; }
    public required IReadOnlyDictionary<string, GenericRecord> SourceSchemasById { get; init; }
    public required IReadOnlyDictionary<string, GenericRecord> LiveSchemasById { get; init; }
    public required IReadOnlyDictionary<string, GenericRecord> SourcePrimaryKeysById { get; init; }
    public required IReadOnlyDictionary<string, GenericRecord> LivePrimaryKeysById { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> SourcePrimaryKeyColumnsByPrimaryKeyId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> LivePrimaryKeyColumnsByPrimaryKeyId { get; init; }
    public required IReadOnlyDictionary<string, GenericRecord> SourceIndexesById { get; init; }
    public required IReadOnlyDictionary<string, GenericRecord> LiveIndexesById { get; init; }
    public required IReadOnlyDictionary<string, GenericRecord> SourceForeignKeysById { get; init; }
    public required IReadOnlyDictionary<string, GenericRecord> LiveForeignKeysById { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> SourceForeignKeysByTargetTableId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> LiveForeignKeysByTargetTableId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> SourceForeignKeyColumnsByForeignKeyId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> LiveForeignKeyColumnsByForeignKeyId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> SourceIndexColumnsByIndexId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> LiveIndexColumnsByIndexId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> SourceColumnDetailsByColumnId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> LiveColumnDetailsByColumnId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> SourcePrimaryKeyColumnsByColumnId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> LivePrimaryKeyColumnsByColumnId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> SourceForeignKeySourceColumnsByColumnId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> LiveForeignKeySourceColumnsByColumnId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> SourceForeignKeyTargetColumnsByColumnId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> LiveForeignKeyTargetColumnsByColumnId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> SourceIndexColumnsByColumnId { get; init; }
    public required IReadOnlyDictionary<string, List<GenericRecord>> LiveIndexColumnsByColumnId { get; init; }
    public required IReadOnlyDictionary<string, List<MetaSqlDifferenceBlocker>> BlockerByColumnPairKey { get; init; }
    public required MetaSqlDestructiveApprovalSet ApprovalSet { get; init; }
    public required IReadOnlySet<string> PlannedAddedTableIds { get; init; }
    public required IReadOnlySet<string> PlannedAddedColumnIds { get; init; }
    public required IReadOnlySet<string> PlannedDroppedForeignKeyIds { get; init; }
    public required IReadOnlySet<string> PlannedAddedForeignKeyIds { get; init; }
}
