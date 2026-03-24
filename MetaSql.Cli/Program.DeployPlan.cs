using MetaSql;
using MetaSql.Extractors.SqlServer;
using Meta.Core.Services;

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

        if (parse.UsedDeprecatedWithDataDrop)
        {
            Presenter.WriteInfo("Warning: --with-data-drop is deprecated for authorization. Use object-scoped approvals.");
        }

        if (parse.UsedDeprecatedWithDataTruncate)
        {
            Presenter.WriteInfo("Warning: --with-data-truncate is deprecated for authorization. Use object-scoped approvals.");
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

            var extractor = new SqlServerMetaSqlExtractor();
            var liveWorkspace = extractor.ExtractMetaSqlWorkspace(new SqlServerExtractRequest
            {
                NewWorkspacePath = liveRuntimePath,
                ConnectionString = parse.ConnectionString,
                SchemaName = parse.SchemaName,
                TableName = parse.TableName,
            });

            var differenceService = new MetaSqlDifferenceService();
            var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);
            var feasibilityBlockers = await new MetaSqlDifferenceFeasibilityService()
                .BuildBlockersAsync(
                    differences,
                    sourceWorkspace,
                    liveWorkspace,
                    parse.ConnectionString)
                .ConfigureAwait(false);

            var manifestService = new MetaSqlDeployManifestService();
            var manifest = manifestService.BuildManifest(
                sourceWorkspace,
                liveWorkspace,
                differences,
                manifestName: "DeployManifest",
                targetDescription: BuildTargetDescription(parse.SchemaName, parse.TableName),
                feasibilityBlockers: feasibilityBlockers,
                destructiveApprovals: parse.DestructiveApprovals);
            await manifest.ManifestModel.SaveToXmlWorkspaceAsync(outputPath).ConfigureAwait(false);

            if (manifest.IsDeployable)
            {
                Presenter.WriteOk(
                    "deploy-plan complete",
                    ("Verdict", "deployable"),
                    ("AddCount", manifest.AddCount.ToString()),
                    ("DropCount", manifest.DropCount.ToString()),
                    ("AlterCount", manifest.AlterCount.ToString()),
                    ("TruncateCount", manifest.TruncateCount.ToString()),
                    ("ReplaceCount", manifest.ReplaceCount.ToString()),
                    ("BlockCount", manifest.BlockCount.ToString()),
                    ("ApprovalCount", parse.DestructiveApprovals.Count.ToString()),
                    ("ManifestPath", outputPath));
                return 0;
            }

            return Fail(
                "deploy-plan produced a non-deployable manifest.",
                "review block entries in the manifest output and fix source/live drift.",
                4,
                RenderManifestIssues(manifest.ManifestModel, outputPath));
        }
        catch (Exception ex)
        {
            return Fail(
                "deploy-plan failed.",
                "check the source workspace, connection string, and any schema/table filters, then retry.",
                4,
                new[]
                {
                    $"  SourceWorkspace: {sourceWorkspacePath}",
                    $"  ManifestPath: {outputPath}",
                    $"  {ex.Message}",
                });
        }
        finally
        {
            DeleteIfExists(tempRootPath);
        }
    }

    private static string BuildTargetDescription(string? schemaName, string? tableName)
    {
        var scope = string.IsNullOrWhiteSpace(schemaName) && string.IsNullOrWhiteSpace(tableName)
            ? "(all)"
            : $"{schemaName ?? "*"}:{tableName ?? "*"}";
        return $"Scope={scope}";
    }

    private static List<string> RenderManifestIssues(
        MetaSqlDeployManifest.MetaSqlDeployManifestModel manifestModel,
        string outputPath)
    {
        var lines = new List<string>
        {
            $"ManifestPath: {outputPath}",
        };

        AddBlockSummaryLines(
            lines,
            manifestModel.BlockTableDifferenceList.Select(row => ("BlockTableDifference", row.DifferenceSummary)));
        AddBlockSummaryLines(
            lines,
            manifestModel.BlockTableColumnDifferenceList.Select(row => ("BlockTableColumnDifference", row.DifferenceSummary)));
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

    private static void PrintDeployPlanHelp()
    {
        Presenter.WriteInfo("Command: deploy-plan");
        Presenter.WriteUsage("meta-sql deploy-plan --source-workspace <path> --connection-string <value> --out <path> [--schema <name>] [--table <name>] [--approve-drop-table <schema.table>] [--approve-drop-column <schema.table.column>] [--approve-truncate-column <schema.table.column>] [--approval-file <path>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads the source MetaSql workspace.");
        Presenter.WriteInfo("  Extracts the live SQL Server schema to MetaSql.");
        Presenter.WriteInfo("  Creates a deploy manifest with Add/Drop/Truncate/Alter/Replace/Block entries.");
        Presenter.WriteInfo("  DataDropTable and DataDropColumn require exact object-scoped approvals.");
        Presenter.WriteInfo("  DataTruncationColumn requires exact object-scoped approval.");
        Presenter.WriteInfo("  --with-data-drop and --with-data-truncate are deprecated compatibility flags and do not grant authorization.");
        Presenter.WriteInfo("  Approvals can be passed as repeated CLI arguments and/or via --approval-file JSON.");
        Presenter.WriteInfo("  Live-only DropPrimaryKey/DropForeignKey/DropIndex are planned by default.");
        Presenter.WriteInfo("  Shared table-column differences become AlterTableColumn when executable and feasible.");
        Presenter.WriteInfo("  Shared primary-key differences become ReplacePrimaryKey when executable; otherwise they are blocked.");
        Presenter.WriteInfo("  Shared foreign-key differences become ReplaceForeignKey when executable; otherwise they are blocked.");
        Presenter.WriteInfo("  Shared index differences become ReplaceIndex when executable; otherwise they are blocked.");
        Presenter.WriteInfo("  Deployable only when BlockCount = 0.");
    }

    private static void DeleteIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
