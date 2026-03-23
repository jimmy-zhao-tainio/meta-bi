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
        var sourceTablesByScopedName = BuildUniqueRecordIndex(
            sourceTablesById.Values,
            row => BuildTableScopeKey(row, sourceSchemasById),
            "source table");
        var liveTablesByScopedName = BuildUniqueRecordIndex(
            liveTablesById.Values,
            row => BuildTableScopeKey(row, liveSchemasById),
            "live table");

        differences.AddRange(
            sourceTablesByScopedName.Keys
                .Except(liveTablesByScopedName.Keys, StringComparer.Ordinal)
                .OrderBy(key => key, StringComparer.Ordinal)
                .Select(key => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.Table,
                    DifferenceKind = MetaSqlDifferenceKind.MissingInLive,
                    DisplayName = FormatTable(sourceTablesByScopedName[key], sourceSchemasById),
                    SourceId = sourceTablesByScopedName[key].Id,
                }));

        differences.AddRange(
            liveTablesByScopedName.Keys
                .Except(sourceTablesByScopedName.Keys, StringComparer.Ordinal)
                .OrderBy(key => key, StringComparer.Ordinal)
                .Select(key => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.Table,
                    DifferenceKind = MetaSqlDifferenceKind.ExtraInLive,
                    DisplayName = FormatTable(liveTablesByScopedName[key], liveSchemasById),
                    LiveId = liveTablesByScopedName[key].Id,
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

        foreach (var tableKey in sourceTablesByScopedName.Keys.Intersect(liveTablesByScopedName.Keys, StringComparer.Ordinal).OrderBy(row => row, StringComparer.Ordinal))
        {
            var sourceTable = sourceTablesByScopedName[tableKey];
            var liveTable = liveTablesByScopedName[tableKey];

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
        var sourceColumnsByName = BuildUniqueRecordIndex(
            sourceColumns.Values,
            row => GetValue(row, "Name"),
            $"source table-column in '{FormatTable(sourceTable, sourceSchemasById)}'");
        var liveColumnsByName = BuildUniqueRecordIndex(
            liveColumns.Values,
            row => GetValue(row, "Name"),
            $"live table-column in '{FormatTable(liveTable, liveSchemasById)}'");

        differences.AddRange(
            sourceColumnsByName.Keys
                .Except(liveColumnsByName.Keys, StringComparer.Ordinal)
                .OrderBy(key => key, StringComparer.Ordinal)
                .Select(key => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.TableColumn,
                    DifferenceKind = MetaSqlDifferenceKind.MissingInLive,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = FormatColumn(sourceColumnsByName[key], sourceTablesById, sourceSchemasById),
                    SourceId = sourceColumnsByName[key].Id,
                }));

        differences.AddRange(
            liveColumnsByName.Keys
                .Except(sourceColumnsByName.Keys, StringComparer.Ordinal)
                .OrderBy(key => key, StringComparer.Ordinal)
                .Select(key => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.TableColumn,
                    DifferenceKind = MetaSqlDifferenceKind.ExtraInLive,
                    ScopeDisplayName = FormatTable(liveTable, liveSchemasById),
                    DisplayName = FormatColumn(liveColumnsByName[key], liveTablesById, liveSchemasById),
                    LiveId = liveColumnsByName[key].Id,
                }));

        foreach (var columnName in sourceColumnsByName.Keys.Intersect(liveColumnsByName.Keys, StringComparer.Ordinal).OrderBy(row => row, StringComparer.Ordinal))
        {
            var sourceColumn = sourceColumnsByName[columnName];
            var liveColumn = liveColumnsByName[columnName];
            if (!AreColumnsEquivalent(sourceColumn, liveColumn, sourceColumnDetailsByColumnId, liveColumnDetailsByColumnId))
            {
                differences.Add(new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.TableColumn,
                    DifferenceKind = MetaSqlDifferenceKind.Different,
                    DisplayName = FormatColumn(sourceColumn, sourceTablesById, sourceSchemasById),
                    SourceId = sourceColumn.Id,
                    LiveId = liveColumn.Id,
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
        var sourcePrimaryKeysByName = BuildUniqueRecordIndex(
            sourcePrimaryKeys.Values,
            row => GetValue(row, "Name"),
            $"source primary-key in '{FormatTable(sourceTable, sourceSchemasById)}'");
        var livePrimaryKeysByName = BuildUniqueRecordIndex(
            livePrimaryKeys.Values,
            row => GetValue(row, "Name"),
            $"live primary-key in '{FormatTable(liveTable, liveSchemasById)}'");

        differences.AddRange(
            sourcePrimaryKeysByName.Keys
                .Except(livePrimaryKeysByName.Keys, StringComparer.Ordinal)
                .OrderBy(key => key, StringComparer.Ordinal)
                .Select(key => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.PrimaryKey,
                    DifferenceKind = MetaSqlDifferenceKind.MissingInLive,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = sourcePrimaryKeysByName[key].Values["Name"],
                    SourceId = sourcePrimaryKeysByName[key].Id,
                }));

        differences.AddRange(
            livePrimaryKeysByName.Keys
                .Except(sourcePrimaryKeysByName.Keys, StringComparer.Ordinal)
                .OrderBy(key => key, StringComparer.Ordinal)
                .Select(key => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.PrimaryKey,
                    DifferenceKind = MetaSqlDifferenceKind.ExtraInLive,
                    ScopeDisplayName = FormatTable(liveTable, liveSchemasById),
                    DisplayName = livePrimaryKeysByName[key].Values["Name"],
                    LiveId = livePrimaryKeysByName[key].Id,
                }));

        foreach (var primaryKeyName in sourcePrimaryKeysByName.Keys.Intersect(livePrimaryKeysByName.Keys, StringComparer.Ordinal).OrderBy(row => row, StringComparer.Ordinal))
        {
            var sourcePrimaryKey = sourcePrimaryKeysByName[primaryKeyName];
            var livePrimaryKey = livePrimaryKeysByName[primaryKeyName];
            if (!ArePrimaryKeysEquivalent(sourcePrimaryKey, livePrimaryKey, sourcePrimaryKeyColumnsByPrimaryKeyId, livePrimaryKeyColumnsByPrimaryKeyId))
            {
                differences.Add(new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.PrimaryKey,
                    DifferenceKind = MetaSqlDifferenceKind.Different,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = sourcePrimaryKey.Values["Name"],
                    SourceId = sourcePrimaryKey.Id,
                    LiveId = livePrimaryKey.Id,
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
        var sourceForeignKeysByName = BuildUniqueRecordIndex(
            sourceForeignKeys.Values,
            row => GetValue(row, "Name"),
            $"source foreign-key in '{FormatTable(sourceTable, sourceSchemasById)}'");
        var liveForeignKeysByName = BuildUniqueRecordIndex(
            liveForeignKeys.Values,
            row => GetValue(row, "Name"),
            $"live foreign-key in '{FormatTable(liveTable, liveSchemasById)}'");

        differences.AddRange(
            sourceForeignKeysByName.Keys
                .Except(liveForeignKeysByName.Keys, StringComparer.Ordinal)
                .OrderBy(key => key, StringComparer.Ordinal)
                .Select(key => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.ForeignKey,
                    DifferenceKind = MetaSqlDifferenceKind.MissingInLive,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = sourceForeignKeysByName[key].Values["Name"],
                    SourceId = sourceForeignKeysByName[key].Id,
                }));

        differences.AddRange(
            liveForeignKeysByName.Keys
                .Except(sourceForeignKeysByName.Keys, StringComparer.Ordinal)
                .OrderBy(key => key, StringComparer.Ordinal)
                .Select(key => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.ForeignKey,
                    DifferenceKind = MetaSqlDifferenceKind.ExtraInLive,
                    ScopeDisplayName = FormatTable(liveTable, liveSchemasById),
                    DisplayName = liveForeignKeysByName[key].Values["Name"],
                    LiveId = liveForeignKeysByName[key].Id,
                }));

        foreach (var foreignKeyName in sourceForeignKeysByName.Keys.Intersect(liveForeignKeysByName.Keys, StringComparer.Ordinal).OrderBy(row => row, StringComparer.Ordinal))
        {
            var sourceForeignKey = sourceForeignKeysByName[foreignKeyName];
            var liveForeignKey = liveForeignKeysByName[foreignKeyName];
            if (!AreForeignKeysEquivalent(sourceForeignKey, liveForeignKey, sourceForeignKeyColumnsByForeignKeyId, liveForeignKeyColumnsByForeignKeyId))
            {
                differences.Add(new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.ForeignKey,
                    DifferenceKind = MetaSqlDifferenceKind.Different,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = sourceForeignKey.Values["Name"],
                    SourceId = sourceForeignKey.Id,
                    LiveId = liveForeignKey.Id,
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
        var sourceIndexesByName = BuildUniqueRecordIndex(
            sourceIndexes.Values,
            row => GetValue(row, "Name"),
            $"source index in '{FormatTable(sourceTable, sourceSchemasById)}'");
        var liveIndexesByName = BuildUniqueRecordIndex(
            liveIndexes.Values,
            row => GetValue(row, "Name"),
            $"live index in '{FormatTable(liveTable, liveSchemasById)}'");

        differences.AddRange(
            sourceIndexesByName.Keys
                .Except(liveIndexesByName.Keys, StringComparer.Ordinal)
                .OrderBy(key => key, StringComparer.Ordinal)
                .Select(key => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.Index,
                    DifferenceKind = MetaSqlDifferenceKind.MissingInLive,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = sourceIndexesByName[key].Values["Name"],
                    SourceId = sourceIndexesByName[key].Id,
                }));

        differences.AddRange(
            liveIndexesByName.Keys
                .Except(sourceIndexesByName.Keys, StringComparer.Ordinal)
                .OrderBy(key => key, StringComparer.Ordinal)
                .Select(key => new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.Index,
                    DifferenceKind = MetaSqlDifferenceKind.ExtraInLive,
                    ScopeDisplayName = FormatTable(liveTable, liveSchemasById),
                    DisplayName = liveIndexesByName[key].Values["Name"],
                    LiveId = liveIndexesByName[key].Id,
                }));

        foreach (var indexName in sourceIndexesByName.Keys.Intersect(liveIndexesByName.Keys, StringComparer.Ordinal).OrderBy(row => row, StringComparer.Ordinal))
        {
            var sourceIndex = sourceIndexesByName[indexName];
            var liveIndex = liveIndexesByName[indexName];
            if (!AreIndexesEquivalent(sourceIndex, liveIndex, sourceIndexColumnsByIndexId, liveIndexColumnsByIndexId))
            {
                differences.Add(new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.Index,
                    DifferenceKind = MetaSqlDifferenceKind.Different,
                    ScopeDisplayName = FormatTable(sourceTable, sourceSchemasById),
                    DisplayName = sourceIndex.Values["Name"],
                    SourceId = sourceIndex.Id,
                    LiveId = liveIndex.Id,
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
        if (!IsSameValue(GetValue(sourceColumn, "Name"), GetValue(liveColumn, "Name")) ||
            !IsSameValue(GetValue(sourceColumn, "Ordinal"), GetValue(liveColumn, "Ordinal")) ||
            !IsSameValue(GetValue(sourceColumn, "MetaDataTypeId"), GetValue(liveColumn, "MetaDataTypeId")) ||
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
        if (!IsSameValue(GetValue(sourcePrimaryKey, "Name"), GetValue(livePrimaryKey, "Name")) ||
            !IsSameValue(GetValue(sourcePrimaryKey, "IsClustered"), GetValue(livePrimaryKey, "IsClustered")))
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
        if (!IsSameValue(GetValue(sourceForeignKey, "Name"), GetValue(liveForeignKey, "Name")) ||
            !IsSameValue(sourceForeignKey.RelationshipIds["TargetTableId"], liveForeignKey.RelationshipIds["TargetTableId"]))
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
        if (!IsSameValue(GetValue(sourceIndex, "Name"), GetValue(liveIndex, "Name")) ||
            !IsSameValue(GetValue(sourceIndex, "IsUnique"), GetValue(liveIndex, "IsUnique")) ||
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

    private static string BuildTableScopeKey(GenericRecord table, IReadOnlyDictionary<string, GenericRecord> schemasById)
    {
        var schemaId = table.RelationshipIds["SchemaId"];
        if (!schemasById.TryGetValue(schemaId, out var schema))
        {
            throw new InvalidOperationException(
                $"Table '{table.Id}' references missing schema '{schemaId}'.");
        }

        return schema.Values["Name"] + "|" + table.Values["Name"];
    }

    private static Dictionary<string, GenericRecord> BuildUniqueRecordIndex(
        IEnumerable<GenericRecord> rows,
        Func<GenericRecord, string> keySelector,
        string contextName)
    {
        var result = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
        foreach (var row in rows)
        {
            var key = keySelector(row);
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException(
                    $"Encountered empty key for {contextName} row '{row.Id}'.");
            }

            if (!result.TryAdd(key, row))
            {
                var existing = result[key];
                throw new InvalidOperationException(
                    $"Ambiguous {contextName} identity key '{key}' between ids '{existing.Id}' and '{row.Id}'.");
            }
        }

        return result;
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
