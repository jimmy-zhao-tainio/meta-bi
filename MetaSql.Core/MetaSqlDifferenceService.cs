using Meta.Adapters;
using Meta.Core.Domain;

namespace MetaSql;

public sealed class MetaSqlDifferenceService
{
    private readonly ServiceCollection _services;

    public MetaSqlDifferenceService()
        : this(new ServiceCollection())
    {
    }

    internal MetaSqlDifferenceService(ServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public async Task<IReadOnlyList<MetaSqlDifference>> BuildDifferencesAsync(
        string sourceWorkspacePath,
        string liveWorkspacePath,
        bool searchUpward = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(liveWorkspacePath);

        var sourceWorkspace = await _services.WorkspaceService
            .LoadAsync(sourceWorkspacePath, searchUpward, cancellationToken)
            .ConfigureAwait(false);
        var liveWorkspace = await _services.WorkspaceService
            .LoadAsync(liveWorkspacePath, searchUpward, cancellationToken)
            .ConfigureAwait(false);

        return BuildDifferences(sourceWorkspace, liveWorkspace);
    }

    public IReadOnlyList<MetaSqlDifference> BuildDifferences(
        Workspace sourceWorkspace,
        Workspace liveWorkspace)
    {
        ArgumentNullException.ThrowIfNull(sourceWorkspace);
        ArgumentNullException.ThrowIfNull(liveWorkspace);

        MetaSqlDiffService.EnsureMetaSqlWorkspace(sourceWorkspace, nameof(sourceWorkspace));
        MetaSqlDiffService.EnsureMetaSqlWorkspace(liveWorkspace, nameof(liveWorkspace));

        var differences = new List<MetaSqlDifference>();

        var sourceSchemasById = GetRecordIndex(sourceWorkspace, "Schema");
        var liveSchemasById = GetRecordIndex(liveWorkspace, "Schema");
        var sourceTablesById = GetRecordIndex(sourceWorkspace, "Table");
        var liveTablesById = GetRecordIndex(liveWorkspace, "Table");

        differences.AddRange(
            sourceTablesById.Keys
                .Except(liveTablesById.Keys, StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal)
                .Select(id => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.Table,
                    DifferenceKind = MetaSqlDifferenceKind.MissingInLive,
                    DisplayName = FormatTable(sourceTablesById[id], sourceSchemasById),
                    SourceId = id,
                }));

        differences.AddRange(
            liveTablesById.Keys
                .Except(sourceTablesById.Keys, StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal)
                .Select(id => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.Table,
                    DifferenceKind = MetaSqlDifferenceKind.ExtraInLive,
                    DisplayName = FormatTable(liveTablesById[id], liveSchemasById),
                    LiveId = id,
                }));

        var sourceColumnsByTableId = GetGroupedRecordIndex(sourceWorkspace, "TableColumn", "TableId");
        var liveColumnsByTableId = GetGroupedRecordIndex(liveWorkspace, "TableColumn", "TableId");
        var sourceColumnDetailsByColumnId = GetGroupedRecords(sourceWorkspace, "TableColumnDataTypeDetail", "TableColumnId");
        var liveColumnDetailsByColumnId = GetGroupedRecords(liveWorkspace, "TableColumnDataTypeDetail", "TableColumnId");

        var sourcePrimaryKeysByTableId = GetGroupedRecordIndex(sourceWorkspace, "PrimaryKey", "TableId");
        var livePrimaryKeysByTableId = GetGroupedRecordIndex(liveWorkspace, "PrimaryKey", "TableId");
        var sourcePrimaryKeyColumnsByPrimaryKeyId = GetGroupedOrderedRecords(sourceWorkspace, "PrimaryKeyColumn", "PrimaryKeyId");
        var livePrimaryKeyColumnsByPrimaryKeyId = GetGroupedOrderedRecords(liveWorkspace, "PrimaryKeyColumn", "PrimaryKeyId");

        var sourceForeignKeysByTableId = GetGroupedRecordIndex(sourceWorkspace, "ForeignKey", "SourceTableId");
        var liveForeignKeysByTableId = GetGroupedRecordIndex(liveWorkspace, "ForeignKey", "SourceTableId");
        var sourceForeignKeyColumnsByForeignKeyId = GetGroupedOrderedRecords(sourceWorkspace, "ForeignKeyColumn", "ForeignKeyId");
        var liveForeignKeyColumnsByForeignKeyId = GetGroupedOrderedRecords(liveWorkspace, "ForeignKeyColumn", "ForeignKeyId");

        var sourceIndexesByTableId = GetGroupedRecordIndex(sourceWorkspace, "Index", "TableId");
        var liveIndexesByTableId = GetGroupedRecordIndex(liveWorkspace, "Index", "TableId");
        var sourceIndexColumnsByIndexId = GetGroupedOrderedRecords(sourceWorkspace, "IndexColumn", "IndexId");
        var liveIndexColumnsByIndexId = GetGroupedOrderedRecords(liveWorkspace, "IndexColumn", "IndexId");

        foreach (var tableId in sourceTablesById.Keys.Intersect(liveTablesById.Keys, StringComparer.Ordinal).OrderBy(row => row, StringComparer.Ordinal))
        {
            var sourceTable = sourceTablesById[tableId];
            var liveTable = liveTablesById[tableId];

            AddColumnDifferences(differences, sourceTable, liveTable, sourceSchemasById, liveSchemasById, sourceTablesById, liveTablesById, sourceColumnsByTableId, liveColumnsByTableId, sourceColumnDetailsByColumnId, liveColumnDetailsByColumnId);
            AddPrimaryKeyDifferences(differences, sourceTable, liveTable, sourceSchemasById, liveSchemasById, sourcePrimaryKeysByTableId, livePrimaryKeysByTableId, sourcePrimaryKeyColumnsByPrimaryKeyId, livePrimaryKeyColumnsByPrimaryKeyId);
            AddForeignKeyDifferences(differences, sourceTable, liveTable, sourceSchemasById, liveSchemasById, sourceForeignKeysByTableId, liveForeignKeysByTableId, sourceForeignKeyColumnsByForeignKeyId, liveForeignKeyColumnsByForeignKeyId);
            AddIndexDifferences(differences, sourceTable, liveTable, sourceSchemasById, liveSchemasById, sourceIndexesByTableId, liveIndexesByTableId, sourceIndexColumnsByIndexId, liveIndexColumnsByIndexId);
        }

        return differences;
    }

    private static void AddColumnDifferences(
        List<MetaSqlDifference> differences,
        GenericRecord sourceTable,
        GenericRecord liveTable,
        IReadOnlyDictionary<string, GenericRecord> sourceSchemasById,
        IReadOnlyDictionary<string, GenericRecord> liveSchemasById,
        IReadOnlyDictionary<string, GenericRecord> sourceTablesById,
        IReadOnlyDictionary<string, GenericRecord> liveTablesById,
        IReadOnlyDictionary<string, Dictionary<string, GenericRecord>> sourceColumnsByTableId,
        IReadOnlyDictionary<string, Dictionary<string, GenericRecord>> liveColumnsByTableId,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceColumnDetailsByColumnId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveColumnDetailsByColumnId)
    {
        var sourceColumns = sourceColumnsByTableId.TryGetValue(sourceTable.Id, out var sourceTableColumns)
            ? sourceTableColumns
            : EmptyRecordIndex();
        var liveColumns = liveColumnsByTableId.TryGetValue(liveTable.Id, out var liveTableColumns)
            ? liveTableColumns
            : EmptyRecordIndex();

        differences.AddRange(
            sourceColumns.Keys
                .Except(liveColumns.Keys, StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal)
                .Select(id => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.TableColumn,
                    DifferenceKind = MetaSqlDifferenceKind.MissingInLive,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = FormatColumn(sourceColumns[id], sourceTablesById, sourceSchemasById),
                    SourceId = id,
                }));

        differences.AddRange(
            liveColumns.Keys
                .Except(sourceColumns.Keys, StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal)
                .Select(id => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.TableColumn,
                    DifferenceKind = MetaSqlDifferenceKind.ExtraInLive,
                    ScopeDisplayName = FormatTable(liveTable, liveSchemasById),
                    DisplayName = FormatColumn(liveColumns[id], liveTablesById, liveSchemasById),
                    LiveId = id,
                }));

        foreach (var columnId in sourceColumns.Keys.Intersect(liveColumns.Keys, StringComparer.Ordinal).OrderBy(row => row, StringComparer.Ordinal))
        {
            var sourceColumn = sourceColumns[columnId];
            var liveColumn = liveColumns[columnId];
            if (!AreColumnsEquivalent(sourceColumn, liveColumn, sourceColumnDetailsByColumnId, liveColumnDetailsByColumnId))
            {
                differences.Add(new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.TableColumn,
                    DifferenceKind = MetaSqlDifferenceKind.Different,
                    DisplayName = FormatColumn(sourceColumn, sourceTablesById, sourceSchemasById),
                    SourceId = columnId,
                    LiveId = columnId,
                });
            }
        }
    }

    private static void AddPrimaryKeyDifferences(
        List<MetaSqlDifference> differences,
        GenericRecord sourceTable,
        GenericRecord liveTable,
        IReadOnlyDictionary<string, GenericRecord> sourceSchemasById,
        IReadOnlyDictionary<string, GenericRecord> liveSchemasById,
        IReadOnlyDictionary<string, Dictionary<string, GenericRecord>> sourcePrimaryKeysByTableId,
        IReadOnlyDictionary<string, Dictionary<string, GenericRecord>> livePrimaryKeysByTableId,
        IReadOnlyDictionary<string, List<GenericRecord>> sourcePrimaryKeyColumnsByPrimaryKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> livePrimaryKeyColumnsByPrimaryKeyId)
    {
        var sourcePrimaryKeys = sourcePrimaryKeysByTableId.TryGetValue(sourceTable.Id, out var sourceTablePrimaryKeys)
            ? sourceTablePrimaryKeys
            : EmptyRecordIndex();
        var livePrimaryKeys = livePrimaryKeysByTableId.TryGetValue(liveTable.Id, out var liveTablePrimaryKeys)
            ? liveTablePrimaryKeys
            : EmptyRecordIndex();

        differences.AddRange(
            sourcePrimaryKeys.Keys
                .Except(livePrimaryKeys.Keys, StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal)
                .Select(id => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.PrimaryKey,
                    DifferenceKind = MetaSqlDifferenceKind.MissingInLive,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = sourcePrimaryKeys[id].Values["Name"],
                    SourceId = id,
                }));

        differences.AddRange(
            livePrimaryKeys.Keys
                .Except(sourcePrimaryKeys.Keys, StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal)
                .Select(id => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.PrimaryKey,
                    DifferenceKind = MetaSqlDifferenceKind.ExtraInLive,
                    ScopeDisplayName = FormatTable(liveTable, liveSchemasById),
                    DisplayName = livePrimaryKeys[id].Values["Name"],
                    LiveId = id,
                }));

        foreach (var primaryKeyId in sourcePrimaryKeys.Keys.Intersect(livePrimaryKeys.Keys, StringComparer.Ordinal).OrderBy(row => row, StringComparer.Ordinal))
        {
            var sourcePrimaryKey = sourcePrimaryKeys[primaryKeyId];
            var livePrimaryKey = livePrimaryKeys[primaryKeyId];
            if (!ArePrimaryKeysEquivalent(sourcePrimaryKey, livePrimaryKey, sourcePrimaryKeyColumnsByPrimaryKeyId, livePrimaryKeyColumnsByPrimaryKeyId))
            {
                differences.Add(new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.PrimaryKey,
                    DifferenceKind = MetaSqlDifferenceKind.Different,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = sourcePrimaryKey.Values["Name"],
                    SourceId = primaryKeyId,
                    LiveId = primaryKeyId,
                });
            }
        }
    }

    private static void AddForeignKeyDifferences(
        List<MetaSqlDifference> differences,
        GenericRecord sourceTable,
        GenericRecord liveTable,
        IReadOnlyDictionary<string, GenericRecord> sourceSchemasById,
        IReadOnlyDictionary<string, GenericRecord> liveSchemasById,
        IReadOnlyDictionary<string, Dictionary<string, GenericRecord>> sourceForeignKeysByTableId,
        IReadOnlyDictionary<string, Dictionary<string, GenericRecord>> liveForeignKeysByTableId,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceForeignKeyColumnsByForeignKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveForeignKeyColumnsByForeignKeyId)
    {
        var sourceForeignKeys = sourceForeignKeysByTableId.TryGetValue(sourceTable.Id, out var sourceTableForeignKeys)
            ? sourceTableForeignKeys
            : EmptyRecordIndex();
        var liveForeignKeys = liveForeignKeysByTableId.TryGetValue(liveTable.Id, out var liveTableForeignKeys)
            ? liveTableForeignKeys
            : EmptyRecordIndex();

        differences.AddRange(
            sourceForeignKeys.Keys
                .Except(liveForeignKeys.Keys, StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal)
                .Select(id => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.ForeignKey,
                    DifferenceKind = MetaSqlDifferenceKind.MissingInLive,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = sourceForeignKeys[id].Values["Name"],
                    SourceId = id,
                }));

        differences.AddRange(
            liveForeignKeys.Keys
                .Except(sourceForeignKeys.Keys, StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal)
                .Select(id => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.ForeignKey,
                    DifferenceKind = MetaSqlDifferenceKind.ExtraInLive,
                    ScopeDisplayName = FormatTable(liveTable, liveSchemasById),
                    DisplayName = liveForeignKeys[id].Values["Name"],
                    LiveId = id,
                }));

        foreach (var foreignKeyId in sourceForeignKeys.Keys.Intersect(liveForeignKeys.Keys, StringComparer.Ordinal).OrderBy(row => row, StringComparer.Ordinal))
        {
            var sourceForeignKey = sourceForeignKeys[foreignKeyId];
            var liveForeignKey = liveForeignKeys[foreignKeyId];
            if (!AreForeignKeysEquivalent(sourceForeignKey, liveForeignKey, sourceForeignKeyColumnsByForeignKeyId, liveForeignKeyColumnsByForeignKeyId))
            {
                differences.Add(new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.ForeignKey,
                    DifferenceKind = MetaSqlDifferenceKind.Different,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = sourceForeignKey.Values["Name"],
                    SourceId = foreignKeyId,
                    LiveId = foreignKeyId,
                });
            }
        }
    }

    private static void AddIndexDifferences(
        List<MetaSqlDifference> differences,
        GenericRecord sourceTable,
        GenericRecord liveTable,
        IReadOnlyDictionary<string, GenericRecord> sourceSchemasById,
        IReadOnlyDictionary<string, GenericRecord> liveSchemasById,
        IReadOnlyDictionary<string, Dictionary<string, GenericRecord>> sourceIndexesByTableId,
        IReadOnlyDictionary<string, Dictionary<string, GenericRecord>> liveIndexesByTableId,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceIndexColumnsByIndexId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveIndexColumnsByIndexId)
    {
        var sourceIndexes = sourceIndexesByTableId.TryGetValue(sourceTable.Id, out var sourceTableIndexes)
            ? sourceTableIndexes
            : EmptyRecordIndex();
        var liveIndexes = liveIndexesByTableId.TryGetValue(liveTable.Id, out var liveTableIndexes)
            ? liveTableIndexes
            : EmptyRecordIndex();

        differences.AddRange(
            sourceIndexes.Keys
                .Except(liveIndexes.Keys, StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal)
                .Select(id => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.Index,
                    DifferenceKind = MetaSqlDifferenceKind.MissingInLive,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = sourceIndexes[id].Values["Name"],
                    SourceId = id,
                }));

        differences.AddRange(
            liveIndexes.Keys
                .Except(sourceIndexes.Keys, StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal)
                .Select(id => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.Index,
                    DifferenceKind = MetaSqlDifferenceKind.ExtraInLive,
                    ScopeDisplayName = FormatTable(liveTable, liveSchemasById),
                    DisplayName = liveIndexes[id].Values["Name"],
                    LiveId = id,
                }));

        foreach (var indexId in sourceIndexes.Keys.Intersect(liveIndexes.Keys, StringComparer.Ordinal).OrderBy(row => row, StringComparer.Ordinal))
        {
            var sourceIndex = sourceIndexes[indexId];
            var liveIndex = liveIndexes[indexId];
            if (!AreIndexesEquivalent(sourceIndex, liveIndex, sourceIndexColumnsByIndexId, liveIndexColumnsByIndexId))
            {
                differences.Add(new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.Index,
                    DifferenceKind = MetaSqlDifferenceKind.Different,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = sourceIndex.Values["Name"],
                    SourceId = indexId,
                    LiveId = indexId,
                });
            }
        }
    }

    private static bool AreColumnsEquivalent(
        GenericRecord sourceColumn,
        GenericRecord liveColumn,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceDetailsByColumnId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveDetailsByColumnId)
    {
        if (!IsSameValue(GetValue(sourceColumn, "MetaDataTypeId"), GetValue(liveColumn, "MetaDataTypeId")) ||
            !IsSameValue(GetValue(sourceColumn, "IsNullable"), GetValue(liveColumn, "IsNullable")) ||
            !IsSameValue(GetValue(sourceColumn, "IsIdentity"), GetValue(liveColumn, "IsIdentity")) ||
            !IsSameValue(GetValue(sourceColumn, "IdentitySeed"), GetValue(liveColumn, "IdentitySeed")) ||
            !IsSameValue(GetValue(sourceColumn, "IdentityIncrement"), GetValue(liveColumn, "IdentityIncrement")) ||
            !IsSameValue(GetValue(sourceColumn, "ExpressionSql"), GetValue(liveColumn, "ExpressionSql")))
        {
            return false;
        }

        var sourceDetails = GetDetailPairs(sourceDetailsByColumnId, sourceColumn.Id);
        var liveDetails = GetDetailPairs(liveDetailsByColumnId, liveColumn.Id);
        return sourceDetails.SequenceEqual(liveDetails);
    }

    private static bool ArePrimaryKeysEquivalent(
        GenericRecord sourcePrimaryKey,
        GenericRecord livePrimaryKey,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceColumnsByPrimaryKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveColumnsByPrimaryKeyId)
    {
        if (!IsSameValue(GetValue(sourcePrimaryKey, "IsClustered"), GetValue(livePrimaryKey, "IsClustered")))
        {
            return false;
        }

        var sourceMembers = GetPrimaryKeyMembers(sourceColumnsByPrimaryKeyId, sourcePrimaryKey.Id);
        var liveMembers = GetPrimaryKeyMembers(liveColumnsByPrimaryKeyId, livePrimaryKey.Id);
        return sourceMembers.SequenceEqual(liveMembers);
    }

    private static bool AreForeignKeysEquivalent(
        GenericRecord sourceForeignKey,
        GenericRecord liveForeignKey,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceColumnsByForeignKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveColumnsByForeignKeyId)
    {
        if (!IsSameValue(sourceForeignKey.RelationshipIds["TargetTableId"], liveForeignKey.RelationshipIds["TargetTableId"]))
        {
            return false;
        }

        var sourceMembers = GetForeignKeyMembers(sourceColumnsByForeignKeyId, sourceForeignKey.Id);
        var liveMembers = GetForeignKeyMembers(liveColumnsByForeignKeyId, liveForeignKey.Id);
        return sourceMembers.SequenceEqual(liveMembers);
    }

    private static bool AreIndexesEquivalent(
        GenericRecord sourceIndex,
        GenericRecord liveIndex,
        IReadOnlyDictionary<string, List<GenericRecord>> sourceColumnsByIndexId,
        IReadOnlyDictionary<string, List<GenericRecord>> liveColumnsByIndexId)
    {
        if (!IsSameValue(GetValue(sourceIndex, "IsUnique"), GetValue(liveIndex, "IsUnique")) ||
            !IsSameValue(GetValue(sourceIndex, "IsClustered"), GetValue(liveIndex, "IsClustered")) ||
            !IsSameValue(GetValue(sourceIndex, "FilterSql"), GetValue(liveIndex, "FilterSql")))
        {
            return false;
        }

        var sourceMembers = GetIndexMembers(sourceColumnsByIndexId, sourceIndex.Id);
        var liveMembers = GetIndexMembers(liveColumnsByIndexId, liveIndex.Id);
        return sourceMembers.SequenceEqual(liveMembers);
    }

    private static List<string> GetDetailPairs(IReadOnlyDictionary<string, List<GenericRecord>> detailsByColumnId, string columnId)
    {
        if (!detailsByColumnId.TryGetValue(columnId, out var rows))
        {
            return [];
        }

        return rows
            .Select(row => $"{GetValue(row, "Name")}={GetValue(row, "Value")}")
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();
    }

    private static List<string> GetPrimaryKeyMembers(IReadOnlyDictionary<string, List<GenericRecord>> rowsByPrimaryKeyId, string primaryKeyId)
    {
        if (!rowsByPrimaryKeyId.TryGetValue(primaryKeyId, out var rows))
        {
            return [];
        }

        return rows
            .OrderBy(row => ParseOrdinal(GetValue(row, "Ordinal")))
            .ThenBy(row => row.Id, StringComparer.Ordinal)
            .Select(row => $"{ParseOrdinal(GetValue(row, "Ordinal"))}:{row.RelationshipIds["TableColumnId"]}:{GetValue(row, "IsDescending")}")
            .ToList();
    }

    private static List<string> GetForeignKeyMembers(IReadOnlyDictionary<string, List<GenericRecord>> rowsByForeignKeyId, string foreignKeyId)
    {
        if (!rowsByForeignKeyId.TryGetValue(foreignKeyId, out var rows))
        {
            return [];
        }

        return rows
            .OrderBy(row => ParseOrdinal(GetValue(row, "Ordinal")))
            .ThenBy(row => row.Id, StringComparer.Ordinal)
            .Select(row => $"{ParseOrdinal(GetValue(row, "Ordinal"))}:{row.RelationshipIds["SourceColumnId"]}:{row.RelationshipIds["TargetColumnId"]}")
            .ToList();
    }

    private static List<string> GetIndexMembers(IReadOnlyDictionary<string, List<GenericRecord>> rowsByIndexId, string indexId)
    {
        if (!rowsByIndexId.TryGetValue(indexId, out var rows))
        {
            return [];
        }

        return rows
            .OrderBy(row => ParseOrdinal(GetValue(row, "Ordinal")))
            .ThenBy(row => row.Id, StringComparer.Ordinal)
            .Select(row => $"{ParseOrdinal(GetValue(row, "Ordinal"))}:{row.RelationshipIds["TableColumnId"]}:{GetValue(row, "IsDescending")}:{GetValue(row, "IsIncluded")}")
            .ToList();
    }

    private static string FormatTable(GenericRecord table, IReadOnlyDictionary<string, GenericRecord> schemasById)
    {
        return $"{schemasById[table.RelationshipIds["SchemaId"]].Values["Name"]}.{table.Values["Name"]}";
    }

    private static string FormatColumn(GenericRecord column, IReadOnlyDictionary<string, GenericRecord> tablesById, IReadOnlyDictionary<string, GenericRecord> schemasById)
    {
        var tableId = column.RelationshipIds["TableId"];
        var table = tablesById[tableId];
        return $"{schemasById[table.RelationshipIds["SchemaId"]].Values["Name"]}.{table.Values["Name"]}.{column.Values["Name"]}";
    }

    private static int ParseOrdinal(string value)
    {
        return int.TryParse(value, out var ordinal) ? ordinal : int.MaxValue;
    }

    private static bool IsSameValue(string? left, string? right) =>
        string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.Ordinal);

    private static string GetValue(GenericRecord record, string propertyName)
    {
        return record.Values.TryGetValue(propertyName, out var value) ? value : string.Empty;
    }

    private static Dictionary<string, GenericRecord> EmptyRecordIndex()
    {
        return new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
    }

    private static Dictionary<string, GenericRecord> GetRecordIndex(Workspace workspace, string entityName)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName).ToDictionary(row => row.Id, StringComparer.Ordinal);
    }

    private static Dictionary<string, Dictionary<string, GenericRecord>> GetGroupedRecordIndex(Workspace workspace, string entityName, string relationshipName)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName)
            .GroupBy(row => row.RelationshipIds[relationshipName], StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.ToDictionary(row => row.Id, StringComparer.Ordinal),
                StringComparer.Ordinal);
    }

    private static Dictionary<string, List<GenericRecord>> GetGroupedRecords(Workspace workspace, string entityName, string relationshipName)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName)
            .GroupBy(row => row.RelationshipIds[relationshipName], StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(row => row.Id, StringComparer.Ordinal).ToList(),
                StringComparer.Ordinal);
    }

    private static Dictionary<string, List<GenericRecord>> GetGroupedOrderedRecords(Workspace workspace, string entityName, string relationshipName)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName)
            .GroupBy(row => row.RelationshipIds[relationshipName], StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(row => ParseOrdinal(GetValue(row, "Ordinal"))).ThenBy(row => row.Id, StringComparer.Ordinal).ToList(),
                StringComparer.Ordinal);
    }
}
