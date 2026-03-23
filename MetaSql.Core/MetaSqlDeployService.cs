using System.Data;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using Meta.Core.Services;
using MetaSqlDeployManifest;
using MetaSql.Extractors.SqlServer;

namespace MetaSql;

public sealed record MetaSqlDeployRequest
{
    public required string ManifestWorkspacePath { get; init; }
    public required string SourceWorkspacePath { get; init; }
    public required string ConnectionString { get; init; }
    public string? SchemaName { get; init; }
    public string? TableName { get; init; }
}

public sealed record MetaSqlDeployResult
{
    public required int AppliedAddCount { get; init; }
    public required int AppliedDropCount { get; init; }
    public required int AppliedAlterCount { get; init; }
    public required int AppliedReplaceCount { get; init; }
    public required int ExecutedStatementCount { get; init; }
}

public sealed class MetaSqlDeployService
{
    private static readonly HashSet<string> ExecutableColumnAspects = new(StringComparer.Ordinal)
    {
        "MetaDataTypeId",
        "MetaDataTypeDetail",
        "IsNullable",
    };

    private static readonly HashSet<string> LengthBasedSqlServerTypeNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "varchar",
        "char",
        "nvarchar",
        "nchar",
        "varbinary",
        "binary",
    };

    private static readonly HashSet<string> SupportedManifestEntityNames =
    [
        "DeployManifest",
        "AddTable",
        "DropTable",
        "AddTableColumn",
        "DropTableColumn",
        "AlterTableColumn",
        "AddPrimaryKey",
        "DropPrimaryKey",
        "ReplacePrimaryKey",
        "AddForeignKey",
        "DropForeignKey",
        "ReplaceForeignKey",
        "AddIndex",
        "DropIndex",
        "ReplaceIndex",
        "BlockTableDifference",
        "BlockTableColumnDifference",
        "BlockPrimaryKeyDifference",
        "BlockForeignKeyDifference",
        "BlockIndexDifference",
    ];

    public async Task<MetaSqlDeployResult> DeployAsync(
        MetaSqlDeployRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ManifestWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.SourceWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ConnectionString);

        var manifestWorkspacePath = Path.GetFullPath(request.ManifestWorkspacePath);
        var sourceWorkspacePath = Path.GetFullPath(request.SourceWorkspacePath);
        var tempRootPath = Path.Combine(Path.GetTempPath(), "MetaSql.Core", "deploy", Guid.NewGuid().ToString("N"));
        var liveWorkspacePath = Path.Combine(tempRootPath, "live-metasql");

        Directory.CreateDirectory(tempRootPath);
        try
        {
            ValidateManifestContract(manifestWorkspacePath);

            var manifestModel = await MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(
                    manifestWorkspacePath,
                    searchUpward: false,
                    cancellationToken)
                .ConfigureAwait(false);
            var root = RequireSingleManifestRoot(manifestModel);

            var blockCount = CountBlocks(manifestModel);
            if (blockCount > 0)
            {
                throw new InvalidOperationException(
                    $"Manifest '{root.Name}' is non-deployable. BlockCount={blockCount}.");
            }

            var workspaceService = new WorkspaceService();
            var sourceWorkspace = await workspaceService
                .LoadAsync(sourceWorkspacePath, searchUpward: false, cancellationToken)
                .ConfigureAwait(false);
            MetaSqlDiffService.EnsureMetaSqlWorkspace(sourceWorkspace, nameof(sourceWorkspace));

            var sourceFingerprint = MetaSqlInstanceFingerprint.Compute(sourceWorkspace);
            if (!string.Equals(sourceFingerprint, root.SourceInstanceFingerprint, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Manifest source fingerprint mismatch. Recreate deploy-plan from the current source workspace.");
            }

            var extractor = new SqlServerMetaSqlExtractor();
            var liveWorkspace = extractor.ExtractMetaSqlWorkspace(new SqlServerExtractRequest
            {
                NewWorkspacePath = liveWorkspacePath,
                ConnectionString = request.ConnectionString,
                SchemaName = request.SchemaName,
                TableName = request.TableName,
            });
            MetaSqlDiffService.EnsureMetaSqlWorkspace(liveWorkspace, nameof(liveWorkspace));

            var liveFingerprint = MetaSqlInstanceFingerprint.Compute(liveWorkspace);
            if (!string.Equals(liveFingerprint, root.LiveInstanceFingerprint, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Manifest live fingerprint mismatch. Live schema changed after deploy-plan. Regenerate the manifest.");
            }

            var sourceModel = await MetaSqlModel.LoadFromXmlWorkspaceAsync(
                    sourceWorkspacePath,
                    searchUpward: false,
                    cancellationToken)
                .ConfigureAwait(false);
            var liveModel = await MetaSqlModel.LoadFromXmlWorkspaceAsync(
                    liveWorkspacePath,
                    searchUpward: false,
                    cancellationToken)
                .ConfigureAwait(false);

            var statements = BuildStatements(manifestModel, sourceModel, liveModel);
            await ExecuteStatementsAsync(request.ConnectionString, statements, cancellationToken).ConfigureAwait(false);

            return new MetaSqlDeployResult
            {
                AppliedAddCount = CountAdds(manifestModel),
                AppliedDropCount = CountDrops(manifestModel),
                AppliedAlterCount = CountAlters(manifestModel),
                AppliedReplaceCount = CountReplaces(manifestModel),
                ExecutedStatementCount = statements.Count,
            };
        }
        finally
        {
            if (Directory.Exists(tempRootPath))
            {
                Directory.Delete(tempRootPath, recursive: true);
            }
        }
    }

    private static DeployManifest RequireSingleManifestRoot(MetaSqlDeployManifestModel manifestModel)
    {
        if (manifestModel.DeployManifestList.Count != 1)
        {
            throw new InvalidOperationException(
                $"Deploy manifest workspace must contain exactly one DeployManifest row. Found {manifestModel.DeployManifestList.Count}.");
        }

        return manifestModel.DeployManifestList[0];
    }

    private static void ValidateManifestContract(string manifestWorkspacePath)
    {
        var modelPath = Path.Combine(manifestWorkspacePath, "metadata", "model.xml");
        if (!File.Exists(modelPath))
        {
            throw new InvalidOperationException(
                $"Deploy manifest metadata model was not found at '{modelPath}'.");
        }

        XDocument document;
        try
        {
            document = XDocument.Load(modelPath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Deploy manifest metadata model could not be parsed at '{modelPath}'.",
                ex);
        }

        var root = document.Root;
        if (root is null)
        {
            throw new InvalidOperationException(
                $"Deploy manifest metadata model is empty at '{modelPath}'.");
        }

        var modelName = GetAttribute(root, "name");
        if (!string.Equals(modelName, "MetaSqlDeployManifest", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Deploy manifest model must be 'MetaSqlDeployManifest'. Actual model: '{modelName}'.");
        }

        var entityList = root.Element("EntityList");
        if (entityList is null)
        {
            throw new InvalidOperationException(
                "Deploy manifest metadata model is missing EntityList.");
        }

        var declaredEntityNames = entityList
            .Elements("Entity")
            .Select(row => GetAttribute(row, "name"))
            .Where(row => !string.IsNullOrWhiteSpace(row))
            .ToHashSet(StringComparer.Ordinal);

        var unsupportedEntityNames = declaredEntityNames
            .Where(row => !SupportedManifestEntityNames.Contains(row))
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();
        if (unsupportedEntityNames.Count > 0)
        {
            throw new InvalidOperationException(
                $"Manifest contains unsupported action kind(s): {string.Join(", ", unsupportedEntityNames)}.");
        }

        var missingRequiredEntityNames = SupportedManifestEntityNames
            .Where(row => !declaredEntityNames.Contains(row))
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();
        if (missingRequiredEntityNames.Count > 0)
        {
            throw new InvalidOperationException(
                $"Manifest is missing required action kind(s): {string.Join(", ", missingRequiredEntityNames)}.");
        }
    }

    private static string GetAttribute(XElement element, string attributeName)
    {
        var value = (string?)element.Attribute(attributeName);
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var alternateName = char.ToUpperInvariant(attributeName[0]) + attributeName[1..];
        return (string?)element.Attribute(alternateName) ?? string.Empty;
    }

    private static int CountAdds(MetaSqlDeployManifestModel manifestModel)
    {
        return manifestModel.AddTableList.Count +
               manifestModel.AddTableColumnList.Count +
               manifestModel.AddPrimaryKeyList.Count +
               manifestModel.AddForeignKeyList.Count +
               manifestModel.AddIndexList.Count;
    }

    private static int CountDrops(MetaSqlDeployManifestModel manifestModel)
    {
        return manifestModel.DropTableList.Count +
               manifestModel.DropTableColumnList.Count +
               manifestModel.DropPrimaryKeyList.Count +
               manifestModel.DropForeignKeyList.Count +
               manifestModel.DropIndexList.Count;
    }

    private static int CountAlters(MetaSqlDeployManifestModel manifestModel)
    {
        return manifestModel.AlterTableColumnList.Count;
    }

    private static int CountReplaces(MetaSqlDeployManifestModel manifestModel)
    {
        return manifestModel.ReplacePrimaryKeyList.Count +
               manifestModel.ReplaceForeignKeyList.Count +
               manifestModel.ReplaceIndexList.Count;
    }

    private static int CountBlocks(MetaSqlDeployManifestModel manifestModel)
    {
        return manifestModel.BlockTableDifferenceList.Count +
               manifestModel.BlockTableColumnDifferenceList.Count +
               manifestModel.BlockPrimaryKeyDifferenceList.Count +
               manifestModel.BlockForeignKeyDifferenceList.Count +
               manifestModel.BlockIndexDifferenceList.Count;
    }

    private static List<string> BuildStatements(
        MetaSqlDeployManifestModel manifestModel,
        MetaSqlModel sourceModel,
        MetaSqlModel liveModel)
    {
        var statements = new List<string>();

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

        var droppedTableIds = manifestModel.DropTableList
            .Select(row => row.LiveTableId)
            .ToHashSet(StringComparer.Ordinal);

        var addedTableIds = manifestModel.AddTableList
            .Select(row => row.SourceTableId)
            .ToHashSet(StringComparer.Ordinal);

        // Drop order removes dependencies before dropping columns/tables.
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

            statements.Add(BuildDropForeignKeySql(foreignKey));
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

            statements.Add(BuildDropIndexSql(index));
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

            statements.Add(BuildDropPrimaryKeySql(primaryKey));
        }

        foreach (var entry in manifestModel.DropTableColumnList
                     .OrderBy(row => row.LiveTableColumnId, StringComparer.Ordinal))
        {
            var column = RequireById(liveColumnsById, entry.LiveTableColumnId, "DropTableColumn.LiveTableColumnId");
            if (droppedTableIds.Contains(column.TableId))
            {
                continue;
            }

            statements.Add(BuildDropColumnSql(column));
        }

        foreach (var tableId in droppedTableIds.OrderBy(row => row, StringComparer.Ordinal))
        {
            var table = RequireById(liveTablesById, tableId, "DropTable.LiveTableId");
            statements.Add(BuildDropTableSql(table));
        }

        foreach (var entry in manifestModel.AlterTableColumnList
                     .OrderBy(row => row.SourceTableColumnId, StringComparer.Ordinal)
                     .ThenBy(row => row.LiveTableColumnId, StringComparer.Ordinal))
        {
            var sourceColumn = RequireById(sourceColumnsById, entry.SourceTableColumnId, "AlterTableColumn.SourceTableColumnId");
            var liveColumn = RequireById(liveColumnsById, entry.LiveTableColumnId, "AlterTableColumn.LiveTableColumnId");
            statements.Add(BuildAlterColumnSql(sourceColumn, liveColumn, sourceColumnDetailsByColumnId, liveColumnDetailsByColumnId));
        }

        // Add order creates tables/columns before keys and indexes.
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

            statements.Add(BuildCreateTableSql(table, columns, sourceColumnDetailsByColumnId));
        }

        foreach (var entry in manifestModel.AddTableColumnList
                     .OrderBy(row => row.SourceTableColumnId, StringComparer.Ordinal))
        {
            var column = RequireById(sourceColumnsById, entry.SourceTableColumnId, "AddTableColumn.SourceTableColumnId");
            if (addedTableIds.Contains(column.TableId))
            {
                continue;
            }

            statements.Add(BuildAddColumnSql(column, sourceColumnDetailsByColumnId));
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

            statements.Add(BuildAddPrimaryKeySql(primaryKey, members));
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

            statements.Add(BuildAddForeignKeySql(foreignKey, members));
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

            statements.Add(BuildAddIndexSql(index, members));
        }

        return statements;
    }

    private static async Task ExecuteStatementsAsync(
        string connectionString,
        IReadOnlyList<string> statements,
        CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        using var transaction = connection.BeginTransaction();
        try
        {
            using (var init = connection.CreateCommand())
            {
                init.Transaction = transaction;
                init.CommandType = CommandType.Text;
                init.CommandText = "SET XACT_ABORT ON;";
                await init.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            for (var i = 0; i < statements.Count; i++)
            {
                var statement = statements[i];
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandType = CommandType.Text;
                command.CommandText = statement;
                try
                {
                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"SQL deploy failed at statement {i + 1}: {statement}",
                        ex);
                }
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static string BuildDropForeignKeySql(ForeignKey foreignKey)
    {
        return $"ALTER TABLE {FormatTableName(foreignKey.SourceTable)} DROP CONSTRAINT {EscapeSqlIdentifier(foreignKey.Name)};";
    }

    private static string BuildDropIndexSql(Index index)
    {
        return $"DROP INDEX {EscapeSqlIdentifier(index.Name)} ON {FormatTableName(index.Table)};";
    }

    private static string BuildDropPrimaryKeySql(PrimaryKey primaryKey)
    {
        return $"ALTER TABLE {FormatTableName(primaryKey.Table)} DROP CONSTRAINT {EscapeSqlIdentifier(primaryKey.Name)};";
    }

    private static string BuildDropColumnSql(TableColumn column)
    {
        return $"ALTER TABLE {FormatTableName(column.Table)} DROP COLUMN {EscapeSqlIdentifier(column.Name)};";
    }

    private static string BuildDropTableSql(Table table)
    {
        return $"DROP TABLE {FormatTableName(table)};";
    }

    private static string BuildCreateTableSql(
        Table table,
        IReadOnlyList<TableColumn> columns,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> detailsByColumnId)
    {
        var columnDefinitions = columns
            .Select(row => BuildColumnDefinition(row, detailsByColumnId))
            .ToList();
        return $"CREATE TABLE {FormatTableName(table)} ({string.Join(", ", columnDefinitions)});";
    }

    private static string BuildAddColumnSql(
        TableColumn column,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> detailsByColumnId)
    {
        return $"ALTER TABLE {FormatTableName(column.Table)} ADD {BuildColumnDefinition(column, detailsByColumnId)};";
    }

    private static string BuildAlterColumnSql(
        TableColumn sourceColumn,
        TableColumn liveColumn,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> sourceDetailsByColumnId,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> liveDetailsByColumnId)
    {
        if (sourceColumn.TableId != liveColumn.TableId)
        {
            throw new InvalidOperationException(
                $"Cannot alter column because source table '{sourceColumn.TableId}' and live table '{liveColumn.TableId}' differ.");
        }

        if (sourceColumn.Name != liveColumn.Name)
        {
            throw new InvalidOperationException(
                $"Cannot alter column because source column name '{sourceColumn.Name}' and live column name '{liveColumn.Name}' differ.");
        }

        if (!string.IsNullOrWhiteSpace(sourceColumn.ExpressionSql))
        {
            throw new InvalidOperationException(
                $"Cannot alter computed column '{sourceColumn.Id}' in this deploy slice.");
        }

        if (IsTrue(sourceColumn.IsIdentity) || IsTrue(liveColumn.IsIdentity))
        {
            throw new InvalidOperationException(
                $"Cannot alter identity column '{sourceColumn.Id}' in this deploy slice.");
        }

        var changedAspects = GetChangedColumnAspects(sourceColumn, liveColumn, sourceDetailsByColumnId, liveDetailsByColumnId);
        if (changedAspects.Count == 0)
        {
            throw new InvalidOperationException(
                $"Cannot alter column '{sourceColumn.Id}' because no supported changed aspects were detected.");
        }

        var unsupportedAspects = changedAspects
            .Where(row => !ExecutableColumnAspects.Contains(row))
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();
        if (unsupportedAspects.Count > 0)
        {
            throw new InvalidOperationException(
                $"Cannot alter column '{sourceColumn.Id}' because unsupported aspect changes are present: {string.Join(", ", unsupportedAspects)}.");
        }

        var typeShapeChanged = changedAspects.Contains("MetaDataTypeId", StringComparer.Ordinal) ||
                               changedAspects.Contains("MetaDataTypeDetail", StringComparer.Ordinal);
        if (typeShapeChanged)
        {
            if (!IsSqlServerTypeId(sourceColumn.MetaDataTypeId) || !IsSqlServerTypeId(liveColumn.MetaDataTypeId))
            {
                throw new InvalidOperationException(
                    $"Cannot alter column '{sourceColumn.Id}' because only sqlserver:type:* MetaDataTypeId values are supported.");
            }

            var sourceTypeName = GetSqlServerTypeName(sourceColumn.MetaDataTypeId);
            var liveTypeName = GetSqlServerTypeName(liveColumn.MetaDataTypeId);
            if (!string.Equals(sourceTypeName, liveTypeName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Cannot alter column '{sourceColumn.Id}' because type-family transitions are blocked in this deploy slice ({liveTypeName} -> {sourceTypeName}).");
            }

            if (!LengthBasedSqlServerTypeNames.Contains(sourceTypeName))
            {
                throw new InvalidOperationException(
                    $"Cannot alter column '{sourceColumn.Id}' because only length-based sqlserver type-shape changes are executable in this deploy slice.");
            }

            var sourceDetailMap = GetDetailMap(sourceDetailsByColumnId, sourceColumn.Id);
            var liveDetailMap = GetDetailMap(liveDetailsByColumnId, liveColumn.Id);
            if (!HasOnlyLengthDetailChange(sourceDetailMap, liveDetailMap))
            {
                throw new InvalidOperationException(
                    $"Cannot alter column '{sourceColumn.Id}' because only Length detail changes are executable for type-shape changes in this deploy slice.");
            }
        }

        var detailValues = GetGroup(sourceDetailsByColumnId, sourceColumn.Id)
            .ToDictionary(row => row.Name, row => row.Value, StringComparer.OrdinalIgnoreCase);
        var typeSql = BuildSqlServerTypeSql(sourceColumn.MetaDataTypeId, detailValues);
        var nullableSql = IsTrue(sourceColumn.IsNullable) ? "NULL" : "NOT NULL";
        return $"ALTER TABLE {FormatTableName(liveColumn.Table)} ALTER COLUMN {EscapeSqlIdentifier(liveColumn.Name)} {typeSql} {nullableSql};";
    }

    private static string BuildAddPrimaryKeySql(PrimaryKey primaryKey, IReadOnlyList<PrimaryKeyColumn> members)
    {
        var clusterClause = IsTrue(primaryKey.IsClustered)
            ? " CLUSTERED"
            : " NONCLUSTERED";
        var memberList = string.Join(
            ", ",
            members.Select(row =>
            {
                var direction = IsTrue(row.IsDescending) ? " DESC" : string.Empty;
                return $"{EscapeSqlIdentifier(row.TableColumn.Name)}{direction}";
            }));
        return $"ALTER TABLE {FormatTableName(primaryKey.Table)} ADD CONSTRAINT {EscapeSqlIdentifier(primaryKey.Name)} PRIMARY KEY{clusterClause} ({memberList});";
    }

    private static string BuildAddForeignKeySql(ForeignKey foreignKey, IReadOnlyList<ForeignKeyColumn> members)
    {
        var sourceColumns = string.Join(", ", members.Select(row => EscapeSqlIdentifier(row.SourceColumn.Name)));
        var targetColumns = string.Join(", ", members.Select(row => EscapeSqlIdentifier(row.TargetColumn.Name)));
        return $"ALTER TABLE {FormatTableName(foreignKey.SourceTable)} ADD CONSTRAINT {EscapeSqlIdentifier(foreignKey.Name)} FOREIGN KEY ({sourceColumns}) REFERENCES {FormatTableName(foreignKey.TargetTable)} ({targetColumns});";
    }

    private static string BuildAddIndexSql(Index index, IReadOnlyList<IndexColumn> members)
    {
        var keyMembers = members
            .Where(row => !IsTrue(row.IsIncluded))
            .ToList();
        if (keyMembers.Count == 0)
        {
            throw new InvalidOperationException($"Cannot add index '{index.Id}' because it has no key members.");
        }

        var includeMembers = members
            .Where(row => IsTrue(row.IsIncluded))
            .ToList();

        var uniqueClause = IsTrue(index.IsUnique) ? "UNIQUE " : string.Empty;
        var clusterClause = IsTrue(index.IsClustered)
            ? "CLUSTERED "
            : IsFalse(index.IsClustered)
                ? "NONCLUSTERED "
                : string.Empty;
        var keys = string.Join(
            ", ",
            keyMembers.Select(row =>
            {
                var direction = IsTrue(row.IsDescending) ? " DESC" : string.Empty;
                return $"{EscapeSqlIdentifier(row.TableColumn.Name)}{direction}";
            }));
        var include = includeMembers.Count == 0
            ? string.Empty
            : $" INCLUDE ({string.Join(", ", includeMembers.Select(row => EscapeSqlIdentifier(row.TableColumn.Name)))})";
        var filter = string.IsNullOrWhiteSpace(index.FilterSql)
            ? string.Empty
            : $" WHERE {index.FilterSql}";
        return $"CREATE {uniqueClause}{clusterClause}INDEX {EscapeSqlIdentifier(index.Name)} ON {FormatTableName(index.Table)} ({keys}){include}{filter};";
    }

    private static string BuildColumnDefinition(
        TableColumn column,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> detailsByColumnId)
    {
        if (!string.IsNullOrWhiteSpace(column.ExpressionSql))
        {
            return $"{EscapeSqlIdentifier(column.Name)} AS {column.ExpressionSql}";
        }

        var detailValues = GetGroup(detailsByColumnId, column.Id)
            .ToDictionary(row => row.Name, row => row.Value, StringComparer.OrdinalIgnoreCase);
        var typeSql = BuildSqlServerTypeSql(column.MetaDataTypeId, detailValues);
        var identitySql = IsTrue(column.IsIdentity)
            ? $" IDENTITY({NormalizeIdentityValue(column.IdentitySeed, "1")},{NormalizeIdentityValue(column.IdentityIncrement, "1")})"
            : string.Empty;
        var nullableSql = IsTrue(column.IsNullable) ? "NULL" : "NOT NULL";
        return $"{EscapeSqlIdentifier(column.Name)} {typeSql}{identitySql} {nullableSql}";
    }

    private static string BuildSqlServerTypeSql(string metaDataTypeId, IReadOnlyDictionary<string, string> detailValues)
    {
        const string prefix = "sqlserver:type:";
        if (string.IsNullOrWhiteSpace(metaDataTypeId) ||
            !metaDataTypeId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Unsupported MetaDataTypeId '{metaDataTypeId}'. Deploy only supports sqlserver:type:*.");
        }

        var typeName = metaDataTypeId[prefix.Length..];
        if (string.IsNullOrWhiteSpace(typeName))
        {
            throw new InvalidOperationException(
                $"Unsupported MetaDataTypeId '{metaDataTypeId}'. Deploy only supports sqlserver:type:*.");
        }

        return typeName.ToLowerInvariant() switch
        {
            "char" or "varchar" or "nchar" or "nvarchar" or "binary" or "varbinary" =>
                $"{typeName}({NormalizeLength(RequireDetail(detailValues, "Length", metaDataTypeId))})",
            "decimal" or "numeric" =>
                BuildNumericTypeSql(typeName, detailValues, metaDataTypeId),
            "time" or "datetime2" or "datetimeoffset" =>
                detailValues.TryGetValue("Precision", out var precision) && !string.IsNullOrWhiteSpace(precision)
                    ? $"{typeName}({precision})"
                    : typeName,
            _ => typeName
        };
    }

    private static string BuildNumericTypeSql(
        string typeName,
        IReadOnlyDictionary<string, string> detailValues,
        string metaDataTypeId)
    {
        var precision = RequireDetail(detailValues, "Precision", metaDataTypeId);
        if (!detailValues.TryGetValue("Scale", out var scale) || string.IsNullOrWhiteSpace(scale))
        {
            return $"{typeName}({precision})";
        }

        return $"{typeName}({precision},{scale})";
    }

    private static string RequireDetail(
        IReadOnlyDictionary<string, string> detailValues,
        string detailName,
        string metaDataTypeId)
    {
        if (!detailValues.TryGetValue(detailName, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"MetaDataTypeId '{metaDataTypeId}' requires detail '{detailName}'.");
        }

        return value;
    }

    private static string NormalizeLength(string value)
    {
        return string.Equals(value, "-1", StringComparison.Ordinal) ? "max" : value;
    }

    private static string NormalizeIdentityValue(string? value, string defaultValue)
    {
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
    }

    private static bool IsSqlServerTypeId(string metaDataTypeId)
    {
        return !string.IsNullOrWhiteSpace(metaDataTypeId) &&
               metaDataTypeId.StartsWith("sqlserver:type:", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetSqlServerTypeName(string metaDataTypeId)
    {
        const string prefix = "sqlserver:type:";
        if (string.IsNullOrWhiteSpace(metaDataTypeId) ||
            !metaDataTypeId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        return metaDataTypeId[prefix.Length..];
    }

    private static List<string> GetChangedColumnAspects(
        TableColumn sourceColumn,
        TableColumn liveColumn,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> sourceDetailsByColumnId,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> liveDetailsByColumnId)
    {
        var changedAspects = new List<string>();
        AddIfDifferent(changedAspects, "Name", sourceColumn.Name, liveColumn.Name);
        AddIfDifferent(changedAspects, "Ordinal", sourceColumn.Ordinal, liveColumn.Ordinal);
        AddIfDifferent(changedAspects, "MetaDataTypeId", sourceColumn.MetaDataTypeId, liveColumn.MetaDataTypeId);
        AddIfDifferent(changedAspects, "IsNullable", sourceColumn.IsNullable, liveColumn.IsNullable);
        AddIfDifferent(changedAspects, "IsIdentity", sourceColumn.IsIdentity, liveColumn.IsIdentity);
        AddIfDifferent(changedAspects, "IdentitySeed", sourceColumn.IdentitySeed, liveColumn.IdentitySeed);
        AddIfDifferent(changedAspects, "IdentityIncrement", sourceColumn.IdentityIncrement, liveColumn.IdentityIncrement);
        AddIfDifferent(changedAspects, "ExpressionSql", sourceColumn.ExpressionSql, liveColumn.ExpressionSql);

        var sourceDetails = GetDetailPairs(sourceDetailsByColumnId, sourceColumn.Id);
        var liveDetails = GetDetailPairs(liveDetailsByColumnId, liveColumn.Id);
        if (!sourceDetails.SequenceEqual(liveDetails))
        {
            changedAspects.Add("MetaDataTypeDetail");
        }

        return changedAspects;
    }

    private static void AddIfDifferent(List<string> changedAspects, string aspectName, string left, string right)
    {
        if (!string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.Ordinal))
        {
            changedAspects.Add(aspectName);
        }
    }

    private static List<string> GetDetailPairs(
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> detailsByColumnId,
        string columnId)
    {
        return GetGroup(detailsByColumnId, columnId)
            .Select(row => $"{row.Name}={row.Value}")
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();
    }

    private static Dictionary<string, string> GetDetailMap(
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> detailsByColumnId,
        string columnId)
    {
        return GetGroup(detailsByColumnId, columnId)
            .ToDictionary(
                row => row.Name,
                row => row.Value,
                StringComparer.OrdinalIgnoreCase);
    }

    private static bool HasOnlyLengthDetailChange(
        IReadOnlyDictionary<string, string> sourceDetailMap,
        IReadOnlyDictionary<string, string> liveDetailMap)
    {
        var detailNames = sourceDetailMap.Keys
            .Concat(liveDetailMap.Keys)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(row => row, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var detailName in detailNames)
        {
            var sourceValue = sourceDetailMap.TryGetValue(detailName, out var sourceFoundValue) ? sourceFoundValue : string.Empty;
            var liveValue = liveDetailMap.TryGetValue(detailName, out var liveFoundValue) ? liveFoundValue : string.Empty;
            if (string.Equals(sourceValue, liveValue, StringComparison.Ordinal))
            {
                continue;
            }

            if (!string.Equals(detailName, "Length", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    private static string FormatTableName(Table table)
    {
        if (table.Schema is null)
        {
            throw new InvalidOperationException($"Table '{table.Id}' is missing Schema relationship.");
        }

        return $"{EscapeSqlIdentifier(table.Schema.Name)}.{EscapeSqlIdentifier(table.Name)}";
    }

    private static string EscapeSqlIdentifier(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return "[" + name.Replace("]", "]]", StringComparison.Ordinal) + "]";
    }

    private static bool IsTrue(string? value)
    {
        return string.Equals(value?.Trim(), "true", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsFalse(string? value)
    {
        return string.Equals(value?.Trim(), "false", StringComparison.OrdinalIgnoreCase);
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
