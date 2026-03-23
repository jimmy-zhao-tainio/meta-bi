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
                withDataDrop: parse.WithDataDrop,
                feasibilityBlockers: feasibilityBlockers);
            await manifest.ManifestModel.SaveToXmlWorkspaceAsync(outputPath).ConfigureAwait(false);

            if (manifest.IsDeployable)
            {
                Presenter.WriteOk(
                    "deploy-plan complete",
                    ("Verdict", "deployable"),
                    ("AddCount", manifest.AddCount.ToString()),
                    ("DropCount", manifest.DropCount.ToString()),
                    ("AlterCount", manifest.AlterCount.ToString()),
                    ("ReplaceCount", manifest.ReplaceCount.ToString()),
                    ("BlockCount", manifest.BlockCount.ToString()),
                    ("IgnoredLiveOnlyDataDropCount", manifest.IgnoredLiveOnlyDataDropCount.ToString()),
                    ("ManifestPath", outputPath));
                return 0;
            }

            return Fail(
                "deploy-plan produced a non-deployable manifest.",
                "review block entries in the manifest output and fix source/live drift.",
                4,
                RenderManifestDifferences(differences, outputPath, parse.WithDataDrop, manifest.IgnoredLiveOnlyDataDropCount));
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

    private static List<string> RenderManifestDifferences(
        IReadOnlyList<MetaSqlDifference> differences,
        string outputPath,
        bool withDataDrop,
        int ignoredLiveOnlyDataDropCount)
    {
        var ignoredLiveOnlyDataDropDifferences = withDataDrop
            ? []
            : differences
                .Where(IsLiveOnlyDataDropDifference)
                .ToList();
        var lines = new List<string>
        {
            $"ManifestPath: {outputPath}",
            $"IgnoredLiveOnlyDataDropCount: {ignoredLiveOnlyDataDropCount}",
        };

        AddMissingOrExtraLines(lines, differences, MetaSqlObjectKind.Table, MetaSqlDifferenceKind.MissingInLive, "AddTable entries:");
        AddMissingOrExtraLines(lines, differences, MetaSqlObjectKind.Table, MetaSqlDifferenceKind.ExtraInLive, "DropTable entries:");
        AddScopedMissingOrExtraLines(lines, differences, MetaSqlObjectKind.TableColumn, MetaSqlDifferenceKind.MissingInLive, "AddTableColumn entries for");
        AddScopedMissingOrExtraLines(lines, differences, MetaSqlObjectKind.TableColumn, MetaSqlDifferenceKind.ExtraInLive, "DropTableColumn entries for");
        AddScopedMissingOrExtraLines(lines, differences, MetaSqlObjectKind.PrimaryKey, MetaSqlDifferenceKind.MissingInLive, "AddPrimaryKey entries for");
        AddScopedMissingOrExtraLines(lines, differences, MetaSqlObjectKind.PrimaryKey, MetaSqlDifferenceKind.ExtraInLive, "DropPrimaryKey entries for");
        AddScopedMissingOrExtraLines(lines, differences, MetaSqlObjectKind.ForeignKey, MetaSqlDifferenceKind.MissingInLive, "AddForeignKey entries for");
        AddScopedMissingOrExtraLines(lines, differences, MetaSqlObjectKind.ForeignKey, MetaSqlDifferenceKind.ExtraInLive, "DropForeignKey entries for");
        AddScopedMissingOrExtraLines(lines, differences, MetaSqlObjectKind.Index, MetaSqlDifferenceKind.MissingInLive, "AddIndex entries for");
        AddScopedMissingOrExtraLines(lines, differences, MetaSqlObjectKind.Index, MetaSqlDifferenceKind.ExtraInLive, "DropIndex entries for");

        AddBlockLines(lines, differences, MetaSqlObjectKind.Table, "BlockTableDifference");
        AddBlockLines(lines, differences, MetaSqlObjectKind.TableColumn, "BlockTableColumnDifference");
        AddBlockLines(lines, differences, MetaSqlObjectKind.PrimaryKey, "BlockPrimaryKeyDifference");
        AddBlockLines(lines, differences, MetaSqlObjectKind.ForeignKey, "BlockForeignKeyDifference");
        AddBlockLines(lines, differences, MetaSqlObjectKind.Index, "BlockIndexDifference");
        AddIgnoredLiveOnlyDataDropLines(lines, ignoredLiveOnlyDataDropDifferences);

        return lines;
    }

    private static void AddMissingOrExtraLines(
        List<string> lines,
        IReadOnlyList<MetaSqlDifference> differences,
        MetaSqlObjectKind objectKind,
        MetaSqlDifferenceKind differenceKind,
        string heading)
    {
        var items = differences
            .Where(row => row.ObjectKind == objectKind && row.DifferenceKind == differenceKind)
            .OrderBy(row => row.DisplayName, StringComparer.Ordinal)
            .Select(row => row.DisplayName)
            .ToList();

        if (items.Count == 0)
        {
            return;
        }

        lines.Add(heading);
        lines.AddRange(items);
    }

    private static void AddScopedMissingOrExtraLines(
        List<string> lines,
        IReadOnlyList<MetaSqlDifference> differences,
        MetaSqlObjectKind objectKind,
        MetaSqlDifferenceKind differenceKind,
        string headingPrefix)
    {
        var groups = differences
            .Where(row => row.ObjectKind == objectKind && row.DifferenceKind == differenceKind)
            .GroupBy(row => row.ScopeDisplayName ?? string.Empty, StringComparer.Ordinal)
            .OrderBy(group => group.Key, StringComparer.Ordinal);

        foreach (var group in groups)
        {
            var items = group
                .OrderBy(row => row.DisplayName, StringComparer.Ordinal)
                .Select(row => "  " + row.DisplayName)
                .ToList();

            if (items.Count == 0)
            {
                continue;
            }

            lines.Add($"{headingPrefix} {group.Key}:");
            lines.AddRange(items);
        }
    }

    private static void AddBlockLines(
        List<string> lines,
        IReadOnlyList<MetaSqlDifference> differences,
        MetaSqlObjectKind objectKind,
        string labelPrefix)
    {
        foreach (var difference in differences
                     .Where(row => row.ObjectKind == objectKind && row.DifferenceKind == MetaSqlDifferenceKind.Different)
                     .OrderBy(row => row.DisplayName, StringComparer.Ordinal))
        {
            lines.Add($"{labelPrefix}: {difference.DisplayName}");
        }
    }

    private static void AddIgnoredLiveOnlyDataDropLines(
        List<string> lines,
        IReadOnlyList<MetaSqlDifference> ignoredLiveOnlyDataDropDifferences)
    {
        if (ignoredLiveOnlyDataDropDifferences.Count == 0)
        {
            return;
        }

        lines.Add("Ignored live-only data-bearing drift entries:");
        foreach (var difference in ignoredLiveOnlyDataDropDifferences
                     .OrderBy(row => row.ObjectKind)
                     .ThenBy(row => row.ScopeDisplayName, StringComparer.Ordinal)
                     .ThenBy(row => row.DisplayName, StringComparer.Ordinal))
        {
            var scope = string.IsNullOrWhiteSpace(difference.ScopeDisplayName)
                ? string.Empty
                : difference.ScopeDisplayName + ".";
            lines.Add($"  {difference.ObjectKind}:{scope}{difference.DisplayName}");
        }
    }

    private static bool IsLiveOnlyDataDropDifference(MetaSqlDifference difference)
    {
        return difference.DifferenceKind == MetaSqlDifferenceKind.ExtraInLive &&
               (difference.ObjectKind == MetaSqlObjectKind.Table ||
                difference.ObjectKind == MetaSqlObjectKind.TableColumn);
    }

    private static void PrintDeployPlanHelp()
    {
        Presenter.WriteInfo("Command: deploy-plan");
        Presenter.WriteUsage("meta-sql deploy-plan --source-workspace <path> --connection-string <value> --out <path> [--schema <name>] [--table <name>] [--with-data-drop]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads the source MetaSql workspace.");
        Presenter.WriteInfo("  Extracts the live SQL Server schema to MetaSql.");
        Presenter.WriteInfo("  Creates a deploy manifest with Add/Drop/Alter/Replace/Block entries.");
        Presenter.WriteInfo("  Without --with-data-drop, live-only Table/TableColumn drift is reported but not planned as drops.");
        Presenter.WriteInfo("  Use --with-data-drop to include DropTable and DropTableColumn actions.");
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
