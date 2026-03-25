namespace MetaSql;

/// <summary>
/// Builds deterministic ordered deploy actions from a validated manifest.
/// </summary>
internal sealed class DeployStatementPlanBuilder
{
    public DeployStatementPlan Build(
        MetaSqlDeployManifest.MetaSqlDeployManifestModel manifestModel,
        MetaSqlModel sourceModel,
        MetaSqlModel liveModel)
    {
        var sourceSchemasById = sourceModel.SchemaList.ToDictionary(row => row.Id, StringComparer.Ordinal);
        var sourceTablesById = sourceModel.TableList.ToDictionary(row => row.Id, StringComparer.Ordinal);
        var sourceColumnsById = sourceModel.TableColumnList.ToDictionary(row => row.Id, StringComparer.Ordinal);
        var sourcePrimaryKeysById = sourceModel.PrimaryKeyList.ToDictionary(row => row.Id, StringComparer.Ordinal);
        var sourceForeignKeysById = sourceModel.ForeignKeyList.ToDictionary(row => row.Id, StringComparer.Ordinal);
        var sourceIndexesById = sourceModel.IndexList.ToDictionary(row => row.Id, StringComparer.Ordinal);

        var sourceColumnsByTableId = GroupBy(sourceModel.TableColumnList, row => row.TableId);
        var sourcePkColumnsByPrimaryKeyId = GroupBy(sourceModel.PrimaryKeyColumnList, row => row.PrimaryKeyId);
        var sourceFkColumnsByForeignKeyId = GroupBy(sourceModel.ForeignKeyColumnList, row => row.ForeignKeyId);
        var sourceIndexColumnsByIndexId = GroupBy(sourceModel.IndexColumnList, row => row.IndexId);
        var sourceColumnDetailsByColumnId = GroupBy(sourceModel.TableColumnDataTypeDetailList, row => row.TableColumnId);

        var liveTablesById = liveModel.TableList.ToDictionary(row => row.Id, StringComparer.Ordinal);
        var liveColumnsById = liveModel.TableColumnList.ToDictionary(row => row.Id, StringComparer.Ordinal);
        var livePrimaryKeysById = liveModel.PrimaryKeyList.ToDictionary(row => row.Id, StringComparer.Ordinal);
        var liveForeignKeysById = liveModel.ForeignKeyList.ToDictionary(row => row.Id, StringComparer.Ordinal);
        var liveIndexesById = liveModel.IndexList.ToDictionary(row => row.Id, StringComparer.Ordinal);
        var liveColumnDetailsByColumnId = GroupBy(liveModel.TableColumnDataTypeDetailList, row => row.TableColumnId);

        var actions = new List<IDeployStatementAction>();
        var droppedTableIds = manifestModel.DropTableList
            .Select(row => row.LiveTableId)
            .ToHashSet(StringComparer.Ordinal);
        var addedTableIds = manifestModel.AddTableList
            .Select(row => row.SourceTableId)
            .ToHashSet(StringComparer.Ordinal);

        var dropForeignKeyIds = manifestModel.DropForeignKeyList
            .Select(row => row.LiveForeignKeyId)
            .Concat(manifestModel.ReplaceForeignKeyList.Select(row => row.LiveForeignKeyId))
            .ToHashSet(StringComparer.Ordinal);
        foreach (var foreignKeyId in dropForeignKeyIds.OrderBy(row => row, StringComparer.Ordinal))
        {
            var foreignKey = RequireById(liveForeignKeysById, foreignKeyId, "DropForeignKey.LiveForeignKeyId");
            if (droppedTableIds.Contains(foreignKey.SourceTableId))
            {
                continue;
            }

            actions.Add(new DropForeignKeyAction(foreignKey));
        }

        var dropIndexIds = manifestModel.DropIndexList
            .Select(row => row.LiveIndexId)
            .Concat(manifestModel.ReplaceIndexList.Select(row => row.LiveIndexId))
            .ToHashSet(StringComparer.Ordinal);
        foreach (var indexId in dropIndexIds.OrderBy(row => row, StringComparer.Ordinal))
        {
            var index = RequireById(liveIndexesById, indexId, "DropIndex.LiveIndexId");
            if (droppedTableIds.Contains(index.TableId))
            {
                continue;
            }

            actions.Add(new DropIndexAction(index));
        }

        var dropPrimaryKeyIds = manifestModel.DropPrimaryKeyList
            .Select(row => row.LivePrimaryKeyId)
            .Concat(manifestModel.ReplacePrimaryKeyList.Select(row => row.LivePrimaryKeyId))
            .ToHashSet(StringComparer.Ordinal);
        foreach (var primaryKeyId in dropPrimaryKeyIds.OrderBy(row => row, StringComparer.Ordinal))
        {
            var primaryKey = RequireById(livePrimaryKeysById, primaryKeyId, "DropPrimaryKey.LivePrimaryKeyId");
            if (droppedTableIds.Contains(primaryKey.TableId))
            {
                continue;
            }

            actions.Add(new DropPrimaryKeyAction(primaryKey));
        }

        foreach (var entry in manifestModel.DropTableColumnList
                     .OrderBy(row => row.LiveTableColumnId, StringComparer.Ordinal))
        {
            var column = RequireById(liveColumnsById, entry.LiveTableColumnId, "DropTableColumn.LiveTableColumnId");
            if (droppedTableIds.Contains(column.TableId))
            {
                continue;
            }

            actions.Add(new DropTableColumnAction(column));
        }

        foreach (var tableId in droppedTableIds.OrderBy(row => row, StringComparer.Ordinal))
        {
            var table = RequireById(liveTablesById, tableId, "DropTable.LiveTableId");
            actions.Add(new DropTableAction(table));
        }

        var alterKeys = manifestModel.AlterTableColumnList
            .Select(row => BuildColumnActionKey(row.SourceTableColumnId, row.LiveTableColumnId))
            .ToHashSet(StringComparer.Ordinal);
        foreach (var entry in manifestModel.TruncateTableColumnDataList
                     .OrderBy(row => row.SourceTableColumnId, StringComparer.Ordinal)
                     .ThenBy(row => row.LiveTableColumnId, StringComparer.Ordinal))
        {
            var actionKey = BuildColumnActionKey(entry.SourceTableColumnId, entry.LiveTableColumnId);
            if (!alterKeys.Contains(actionKey))
            {
                throw new InvalidOperationException(
                    $"Manifest truncation action '{entry.Id}' has no matching AlterTableColumn action.");
            }

            var sourceColumn = RequireById(sourceColumnsById, entry.SourceTableColumnId, "TruncateTableColumnData.SourceTableColumnId");
            var liveColumn = RequireById(liveColumnsById, entry.LiveTableColumnId, "TruncateTableColumnData.LiveTableColumnId");
            actions.Add(new TruncateTableColumnDataAction(sourceColumn, liveColumn, sourceColumnDetailsByColumnId));
        }

        foreach (var entry in manifestModel.AlterTableColumnList
                     .OrderBy(row => row.SourceTableColumnId, StringComparer.Ordinal)
                     .ThenBy(row => row.LiveTableColumnId, StringComparer.Ordinal))
        {
            var sourceColumn = RequireById(sourceColumnsById, entry.SourceTableColumnId, "AlterTableColumn.SourceTableColumnId");
            var liveColumn = RequireById(liveColumnsById, entry.LiveTableColumnId, "AlterTableColumn.LiveTableColumnId");
            actions.Add(new AlterTableColumnAction(sourceColumn, liveColumn, sourceColumnDetailsByColumnId, liveColumnDetailsByColumnId));
        }

        foreach (var entry in manifestModel.AddSchemaList
                     .OrderBy(row => row.SourceSchemaId, StringComparer.Ordinal))
        {
            var schema = RequireById(sourceSchemasById, entry.SourceSchemaId, "AddSchema.SourceSchemaId");
            actions.Add(new AddSchemaAction(schema));
        }

        foreach (var tableId in addedTableIds.OrderBy(row => row, StringComparer.Ordinal))
        {
            var table = RequireById(sourceTablesById, tableId, "AddTable.SourceTableId");
            var columns = GetGroup(sourceColumnsByTableId, table.Id)
                .OrderBy(row => ParseOrdinal(row.Ordinal))
                .ThenBy(row => row.Name, StringComparer.Ordinal)
                .ToList();
            if (columns.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Cannot add table '{table.Id}' because no source columns were found for it.");
            }

            actions.Add(new AddTableAction(table, columns, sourceColumnDetailsByColumnId));
        }

        foreach (var entry in manifestModel.AddTableColumnList
                     .OrderBy(row => row.SourceTableColumnId, StringComparer.Ordinal))
        {
            var column = RequireById(sourceColumnsById, entry.SourceTableColumnId, "AddTableColumn.SourceTableColumnId");
            if (addedTableIds.Contains(column.TableId))
            {
                continue;
            }

            actions.Add(new AddTableColumnAction(column, sourceColumnDetailsByColumnId));
        }

        var addPrimaryKeyIds = manifestModel.AddPrimaryKeyList
            .Select(row => row.SourcePrimaryKeyId)
            .Concat(manifestModel.ReplacePrimaryKeyList.Select(row => row.SourcePrimaryKeyId))
            .ToHashSet(StringComparer.Ordinal);
        foreach (var primaryKeyId in addPrimaryKeyIds.OrderBy(row => row, StringComparer.Ordinal))
        {
            var primaryKey = RequireById(sourcePrimaryKeysById, primaryKeyId, "AddPrimaryKey.SourcePrimaryKeyId");
            var members = GetGroup(sourcePkColumnsByPrimaryKeyId, primaryKey.Id)
                .OrderBy(row => ParseOrdinal(row.Ordinal))
                .ThenBy(row => row.Id, StringComparer.Ordinal)
                .ToList();
            if (members.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Cannot add primary key '{primaryKey.Id}' because no source key members were found.");
            }

            actions.Add(new AddPrimaryKeyAction(primaryKey, members));
        }

        var addForeignKeyIds = manifestModel.AddForeignKeyList
            .Select(row => row.SourceForeignKeyId)
            .Concat(manifestModel.ReplaceForeignKeyList.Select(row => row.SourceForeignKeyId))
            .ToHashSet(StringComparer.Ordinal);
        foreach (var foreignKeyId in addForeignKeyIds.OrderBy(row => row, StringComparer.Ordinal))
        {
            var foreignKey = RequireById(sourceForeignKeysById, foreignKeyId, "AddForeignKey.SourceForeignKeyId");
            var members = GetGroup(sourceFkColumnsByForeignKeyId, foreignKey.Id)
                .OrderBy(row => ParseOrdinal(row.Ordinal))
                .ThenBy(row => row.Id, StringComparer.Ordinal)
                .ToList();
            if (members.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Cannot add foreign key '{foreignKey.Id}' because no source key members were found.");
            }

            actions.Add(new AddForeignKeyAction(foreignKey, members));
        }

        var addIndexIds = manifestModel.AddIndexList
            .Select(row => row.SourceIndexId)
            .Concat(manifestModel.ReplaceIndexList.Select(row => row.SourceIndexId))
            .ToHashSet(StringComparer.Ordinal);
        foreach (var indexId in addIndexIds.OrderBy(row => row, StringComparer.Ordinal))
        {
            var index = RequireById(sourceIndexesById, indexId, "AddIndex.SourceIndexId");
            var members = GetGroup(sourceIndexColumnsByIndexId, index.Id)
                .OrderBy(row => ParseOrdinal(row.Ordinal))
                .ThenBy(row => row.Id, StringComparer.Ordinal)
                .ToList();
            if (members.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Cannot add index '{index.Id}' because no source index members were found.");
            }

            actions.Add(new AddIndexAction(index, members));
        }

        return new DeployStatementPlan
        {
            ManifestModel = manifestModel,
            SourceModel = sourceModel,
            LiveModel = liveModel,
            AppliedAddCount = CountAdds(manifestModel),
            AppliedDropCount = CountDrops(manifestModel),
            AppliedAlterCount = CountAlters(manifestModel),
            AppliedTruncateCount = CountTruncates(manifestModel),
            AppliedReplaceCount = CountReplaces(manifestModel),
            Actions = actions,
        };
    }

    public int CountBlocks(MetaSqlDeployManifest.MetaSqlDeployManifestModel manifestModel)
    {
        return manifestModel.BlockTableDifferenceList.Count +
               manifestModel.BlockTableColumnDifferenceList.Count +
               manifestModel.BlockPrimaryKeyDifferenceList.Count +
               manifestModel.BlockForeignKeyDifferenceList.Count +
               manifestModel.BlockIndexDifferenceList.Count;
    }

    private static int CountAdds(MetaSqlDeployManifest.MetaSqlDeployManifestModel manifestModel)
    {
        return manifestModel.AddSchemaList.Count +
               manifestModel.AddTableList.Count +
               manifestModel.AddTableColumnList.Count +
               manifestModel.AddPrimaryKeyList.Count +
               manifestModel.AddForeignKeyList.Count +
               manifestModel.AddIndexList.Count;
    }

    private static int CountDrops(MetaSqlDeployManifest.MetaSqlDeployManifestModel manifestModel)
    {
        return manifestModel.DropTableList.Count +
               manifestModel.DropTableColumnList.Count +
               manifestModel.DropPrimaryKeyList.Count +
               manifestModel.DropForeignKeyList.Count +
               manifestModel.DropIndexList.Count;
    }

    private static int CountAlters(MetaSqlDeployManifest.MetaSqlDeployManifestModel manifestModel)
    {
        return manifestModel.AlterTableColumnList.Count;
    }

    private static int CountTruncates(MetaSqlDeployManifest.MetaSqlDeployManifestModel manifestModel)
    {
        return manifestModel.TruncateTableColumnDataList.Count;
    }

    private static int CountReplaces(MetaSqlDeployManifest.MetaSqlDeployManifestModel manifestModel)
    {
        return manifestModel.ReplacePrimaryKeyList.Count +
               manifestModel.ReplaceForeignKeyList.Count +
               manifestModel.ReplaceIndexList.Count;
    }

    private static string BuildColumnActionKey(string sourceTableColumnId, string liveTableColumnId)
    {
        return sourceTableColumnId + "|" + liveTableColumnId;
    }

    private static int ParseOrdinal(string? value)
    {
        return int.TryParse(value, out var ordinal) ? ordinal : int.MaxValue;
    }

    private static Dictionary<string, List<T>> GroupBy<T>(IEnumerable<T> rows, Func<T, string> keySelector)
    {
        var groups = new Dictionary<string, List<T>>(StringComparer.Ordinal);
        foreach (var row in rows)
        {
            var key = keySelector(row);
            if (!groups.TryGetValue(key, out var bucket))
            {
                bucket = new List<T>();
                groups[key] = bucket;
            }

            bucket.Add(row);
        }

        return groups;
    }

    private static IReadOnlyList<T> GetGroup<T>(IReadOnlyDictionary<string, List<T>> groups, string key)
    {
        return groups.TryGetValue(key, out var bucket)
            ? bucket
            : Array.Empty<T>();
    }

    private static T RequireById<T>(IReadOnlyDictionary<string, T> rowsById, string id, string fieldName)
        where T : class
    {
        if (!rowsById.TryGetValue(id, out var value))
        {
            throw new InvalidOperationException(
                $"Manifest references unknown id '{id}' in '{fieldName}'.");
        }

        return value;
    }
}
