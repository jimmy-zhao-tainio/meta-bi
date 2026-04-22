using MetaSql;
using MetaSql.Extractors.SqlServer;
using Meta.Core.Connections;
using Meta.Core.Services;
using Meta.Core.Domain;

internal static partial class Program
{
    private static async Task<int> RunDeployPlanAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintDeployPlanHelp();
            return 0;
        }

        var parse = ParseDiffLikeArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, "meta-sql deploy-plan --help");
        }

        if (string.IsNullOrWhiteSpace(parse.OutputPath))
        {
            return Fail("missing required option --out <path>.", "meta-sql deploy-plan --help");
        }

        string connectionString;
        try
        {
            connectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
                parse.ConnectionEnvironmentVariableName);
        }
        catch (ConnectionEnvironmentVariableException exception)
        {
            return Fail(exception.Message, "meta-sql deploy-plan --help");
        }

        var sourceWorkspacePath = Path.GetFullPath(parse.SourceWorkspacePath);
        var outputPath = Path.GetFullPath(parse.OutputPath);
        var tempRootPath = Path.Combine(Path.GetTempPath(), "MetaSql.Cli", "deploy-plan", Guid.NewGuid().ToString("N"));
        var liveRuntimePath = Path.Combine(tempRootPath, "live-metasql");

        try
        {
            Directory.CreateDirectory(tempRootPath);

            var workspaceService = new WorkspaceService();
            var sourceWorkspace = await workspaceService.LoadAsync(sourceWorkspacePath, searchUpward: false).ConfigureAwait(false);

            var liveDatabasePresence = await SqlServerDatabaseRuntime
                .GetPresenceAsync(connectionString)
                .ConfigureAwait(false);
            Workspace liveWorkspace;
            if (liveDatabasePresence == MetaSqlLiveDatabasePresence.Missing)
            {
                liveWorkspace = SqlServerMetaSqlWorkspaceFactory.CreateEmptyWorkspace(
                    liveRuntimePath,
                    SqlServerDatabaseRuntime.RequireDatabaseName(connectionString));
            }
            else
            {
                var extractor = new SqlServerMetaSqlExtractor();
                liveWorkspace = extractor.ExtractMetaSqlWorkspace(new SqlServerExtractRequest
                {
                    NewWorkspacePath = liveRuntimePath,
                    ConnectionString = connectionString,
                    AllowEmpty = true,
                });
            }

            var differenceService = new MetaSqlDifferenceService();
            var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);
            var feasibilityBlockers = await new MetaSqlDifferenceFeasibilityService()
                .BuildBlockersAsync(
                    differences,
                    sourceWorkspace,
                    liveWorkspace,
                    connectionString)
                .ConfigureAwait(false);

            var manifestService = new MetaSqlDeployManifestService();
            var manifest = manifestService.BuildManifest(
                sourceWorkspace,
                liveWorkspace,
                liveDatabasePresence,
                differences,
                manifestName: "DeployManifest",
                targetDescription: BuildTargetDescription(),
                feasibilityBlockers: feasibilityBlockers,
                destructiveApprovals: parse.DestructiveApprovals);
            await manifest.ManifestModel.SaveToXmlWorkspaceAsync(outputPath).ConfigureAwait(false);

            if (manifest.IsDeployable)
            {
                var details = new List<(string Label, string Value)>
                {
                    ("Status", "ready to deploy"),
                    ("Changes", FormatManifestChangeSummary(manifest.ManifestModel)),
                };
                if (parse.DestructiveApprovals.Count > 0)
                {
                    details.Add(("Approvals used", parse.DestructiveApprovals.Count.ToString()));
                }

                Presenter.WriteOk("Created deploy plan", details.ToArray());
                return 0;
            }

            return Fail(
                "deploy-plan produced a non-deployable manifest.",
                "review block entries in the manifest output and fix source/live drift.",
                4,
                RenderManifestIssues(manifest.ManifestModel, outputPath, sourceWorkspace, liveWorkspace));
        }
        catch (Exception ex)
        {
            return Fail(
                "deploy-plan failed.",
                "check the source workspace and connection string, then retry.",
                4,
                new[]
                {
                    $"  SourceWorkspace: {sourceWorkspacePath}",
                    $"  ManifestPath: {outputPath}",
                    $"  {ConnectionEnvironmentVariableResolver.FormatReference(parse.ConnectionEnvironmentVariableName)}",
                    $"  {ex.Message}",
                });
        }
        finally
        {
            DeleteIfExists(tempRootPath);
        }
    }

    private static string BuildTargetDescription() => "Scope=(all)";

    private static List<string> RenderManifestIssues(
        MetaSqlDeployManifest.MetaSqlDeployManifestModel manifestModel,
        string outputPath,
        Workspace sourceWorkspace,
        Workspace liveWorkspace)
    {
        var lines = new List<string>
        {
            $"ManifestPath: {outputPath}",
        };

        AddBlockSummaryLines(
            lines,
            manifestModel.BlockTableDifferenceList.Select(row => ("BlockTableDifference", row.DifferenceSummary)));
        AddTableColumnBlockLines(lines, manifestModel.BlockTableColumnDifferenceList, sourceWorkspace, liveWorkspace);
        AddBlockSummaryLines(
            lines,
            manifestModel.BlockPrimaryKeyDifferenceList.Select(row => ("BlockPrimaryKeyDifference", row.DifferenceSummary)));
        AddBlockSummaryLines(
            lines,
            manifestModel.BlockForeignKeyDifferenceList.Select(row => ("BlockForeignKeyDifference", row.DifferenceSummary)));
        AddBlockSummaryLines(
            lines,
            manifestModel.BlockIndexDifferenceList.Select(row => ("BlockIndexDifference", row.DifferenceSummary)));

        return lines;
    }

    private static void AddTableColumnBlockLines(
        List<string> lines,
        IEnumerable<MetaSqlDeployManifest.BlockTableColumnDifference> rows,
        Workspace sourceWorkspace,
        Workspace liveWorkspace)
    {
        var sourceColumnsById = BuildRecordIndex(sourceWorkspace, "TableColumn");
        var liveColumnsById = BuildRecordIndex(liveWorkspace, "TableColumn");
        var sourceTablesById = BuildRecordIndex(sourceWorkspace, "Table");
        var liveTablesById = BuildRecordIndex(liveWorkspace, "Table");
        var sourceSchemasById = BuildRecordIndex(sourceWorkspace, "Schema");
        var liveSchemasById = BuildRecordIndex(liveWorkspace, "Schema");
        var sourceDetailsByColumnId = BuildGroupedRecordIndex(sourceWorkspace, "TableColumnDataTypeDetail", "TableColumnId");
        var liveDetailsByColumnId = BuildGroupedRecordIndex(liveWorkspace, "TableColumnDataTypeDetail", "TableColumnId");

        foreach (var row in rows
                     .OrderBy(item => item.SourceTableColumnId, StringComparer.Ordinal)
                     .ThenBy(item => item.LiveTableColumnId, StringComparer.Ordinal)
                     .ThenBy(item => item.DifferenceSummary, StringComparer.Ordinal))
        {
            sourceColumnsById.TryGetValue(row.SourceTableColumnId, out var sourceColumn);
            liveColumnsById.TryGetValue(row.LiveTableColumnId, out var liveColumn);

            var displayName = FormatBlockedColumnName(
                sourceColumn,
                liveColumn,
                sourceTablesById,
                liveTablesById,
                sourceSchemasById,
                liveSchemasById,
                row.DifferenceSummary);

            lines.Add($"Blocked column: {displayName}");

            if (sourceColumn is not null)
            {
                lines.Add($"  Source: {FormatColumnShape(sourceColumn, sourceDetailsByColumnId)}");
            }

            if (liveColumn is not null)
            {
                lines.Add($"  Live: {FormatColumnShape(liveColumn, liveDetailsByColumnId)}");
            }

            lines.Add($"  Why blocked: {TrimDisplayPrefix(row.DifferenceSummary, displayName)}");
        }
    }

    private static void AddBlockSummaryLines(
        List<string> lines,
        IEnumerable<(string Label, string Summary)> rows)
    {
        var items = rows
            .OrderBy(row => row.Label, StringComparer.Ordinal)
            .ThenBy(row => row.Summary, StringComparer.Ordinal)
            .ToList();

        if (items.Count == 0)
        {
            return;
        }

        foreach (var item in items)
        {
            lines.Add($"{item.Label}: {item.Summary}");
        }
    }

    private static Dictionary<string, GenericRecord> BuildRecordIndex(Workspace workspace, string entityName)
    {
        return workspace.Instance
            .GetOrCreateEntityRecords(entityName)
            .ToDictionary(row => row.Id, StringComparer.Ordinal);
    }

    private static Dictionary<string, List<GenericRecord>> BuildGroupedRecordIndex(
        Workspace workspace,
        string entityName,
        string relationshipName)
    {
        return workspace.Instance
            .GetOrCreateEntityRecords(entityName)
            .Where(row => row.RelationshipIds.TryGetValue(relationshipName, out var id) && !string.IsNullOrWhiteSpace(id))
            .GroupBy(row => row.RelationshipIds[relationshipName], StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);
    }

    private static string FormatBlockedColumnName(
        GenericRecord? sourceColumn,
        GenericRecord? liveColumn,
        IReadOnlyDictionary<string, GenericRecord> sourceTablesById,
        IReadOnlyDictionary<string, GenericRecord> liveTablesById,
        IReadOnlyDictionary<string, GenericRecord> sourceSchemasById,
        IReadOnlyDictionary<string, GenericRecord> liveSchemasById,
        string fallback)
    {
        if (sourceColumn is not null)
        {
            return FormatColumnName(sourceColumn, sourceTablesById, sourceSchemasById);
        }

        if (liveColumn is not null)
        {
            return FormatColumnName(liveColumn, liveTablesById, liveSchemasById);
        }

        var separator = fallback.IndexOf(':');
        return separator >= 0
            ? fallback[..separator].Trim()
            : fallback.Trim();
    }

    private static string FormatColumnName(
        GenericRecord column,
        IReadOnlyDictionary<string, GenericRecord> tablesById,
        IReadOnlyDictionary<string, GenericRecord> schemasById)
    {
        if (!column.RelationshipIds.TryGetValue("TableId", out var tableId) ||
            !tablesById.TryGetValue(tableId, out var table))
        {
            return column.Values.GetValueOrDefault("Name") ?? column.Id;
        }

        if (!table.RelationshipIds.TryGetValue("SchemaId", out var schemaId) ||
            !schemasById.TryGetValue(schemaId, out var schema))
        {
            return $"{table.Values.GetValueOrDefault("Name")}.{column.Values.GetValueOrDefault("Name")}";
        }

        return $"{schema.Values.GetValueOrDefault("Name")}.{table.Values.GetValueOrDefault("Name")}.{column.Values.GetValueOrDefault("Name")}";
    }

    private static string FormatColumnShape(
        GenericRecord column,
        IReadOnlyDictionary<string, List<GenericRecord>> detailsByColumnId)
    {
        var typeName = GetSqlServerTypeName(column.Values.GetValueOrDefault("MetaDataTypeId"));
        var detailMap = BuildNormalizedDetailMap(detailsByColumnId, column.Id);
        var shape = BuildTypeShape(typeName, detailMap);
        var isNullable = string.Equals(column.Values.GetValueOrDefault("IsNullable"), "true", StringComparison.OrdinalIgnoreCase);
        return isNullable
            ? shape + " null"
            : shape + " not null";
    }

    private static string BuildTypeShape(string typeName, IReadOnlyDictionary<string, string> detailMap)
    {
        if (detailMap.TryGetValue("Length", out var length) && !string.IsNullOrWhiteSpace(length))
        {
            var renderedLength = string.Equals(length, "-1", StringComparison.Ordinal) ? "max" : length;
            return $"{typeName}({renderedLength})";
        }

        if (detailMap.TryGetValue("Precision", out var precision) && !string.IsNullOrWhiteSpace(precision))
        {
            if (detailMap.TryGetValue("Scale", out var scale) && !string.IsNullOrWhiteSpace(scale))
            {
                return $"{typeName}({precision},{scale})";
            }

            return $"{typeName}({precision})";
        }

        return typeName;
    }

    private static Dictionary<string, string> BuildNormalizedDetailMap(
        IReadOnlyDictionary<string, List<GenericRecord>> detailsByColumnId,
        string columnId)
    {
        if (!detailsByColumnId.TryGetValue(columnId, out var rows))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        var map = rows.ToDictionary(
            row => row.Values.GetValueOrDefault("Name") ?? string.Empty,
            row => row.Values.GetValueOrDefault("Value") ?? string.Empty,
            StringComparer.OrdinalIgnoreCase);

        if (!map.ContainsKey("Precision") &&
            map.TryGetValue("NumericPrecision", out var numericPrecision) &&
            !string.IsNullOrWhiteSpace(numericPrecision))
        {
            map["Precision"] = numericPrecision;
        }

        return map;
    }

    private static string GetSqlServerTypeName(string? metaDataTypeId)
    {
        if (string.IsNullOrWhiteSpace(metaDataTypeId))
        {
            return "(unknown type)";
        }

        var separator = metaDataTypeId.LastIndexOf(':');
        return separator >= 0 && separator + 1 < metaDataTypeId.Length
            ? metaDataTypeId[(separator + 1)..]
            : metaDataTypeId;
    }

    private static string TrimDisplayPrefix(string summary, string displayName)
    {
        var prefix = displayName + ":";
        return summary.StartsWith(prefix, StringComparison.Ordinal)
            ? summary[prefix.Length..].TrimStart()
            : summary;
    }

    private static void PrintDeployPlanHelp()
    {
        Presenter.WriteInfo("Command: deploy-plan");
        Presenter.WriteUsage("meta-sql deploy-plan --source-workspace <path> --connection-env <name> --out <path> [--approve-drop-table <schema.table>] [--approve-drop-column <schema.table.column>] [--approve-truncate-column <schema.table.column>] [--approval-file <path>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads the source MetaSql workspace.");
        Presenter.WriteInfo("  Extracts the live SQL Server schema to MetaSql.");
        Presenter.WriteInfo("  --connection-env names the environment variable that contains the SQL Server connection string.");
        Presenter.WriteInfo("  Always plans against the full source workspace and full live database. Filtered subset deploy is not supported.");
        Presenter.WriteInfo("  Creates a deploy manifest with Add/Drop/Truncate/Alter/Replace/Block entries.");
        Presenter.WriteInfo("  DataDropTable and DataDropColumn require exact object-scoped approvals.");
        Presenter.WriteInfo("  DataTruncationColumn requires exact object-scoped approval.");
        Presenter.WriteInfo("  Approvals can be passed as repeated CLI arguments and/or via --approval-file JSON.");
        Presenter.WriteInfo("  Live-only DropPrimaryKey/DropForeignKey/DropIndex are planned by default.");
        Presenter.WriteInfo("  Shared table-column differences become AlterTableColumn when executable and feasible.");
        Presenter.WriteInfo("  Shared primary-key differences become ReplacePrimaryKey when executable; otherwise they are blocked.");
        Presenter.WriteInfo("  Shared foreign-key differences become ReplaceForeignKey when executable; otherwise they are blocked.");
        Presenter.WriteInfo("  Shared index differences become ReplaceIndex when executable; otherwise they are blocked.");
        Presenter.WriteInfo("  Deployable only when there are no block entries.");
    }

    private static void DeleteIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
