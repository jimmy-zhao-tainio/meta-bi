using Meta.Core.Presentation;
using MetaConvert.SchemaToDataVault;
using MetaRawDataVault.Instance;
using MetaSchema.Instance;

internal static class Program
{
    private static readonly ConsolePresenter Presenter = new();

    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return 0;
        }

        if (string.Equals(args[0], "schema-to-raw-datavault", StringComparison.OrdinalIgnoreCase))
        {
            return await RunSchemaToRawDataVaultAsync(args).ConfigureAwait(false);
        }

        return Fail($"unknown command '{args[0]}'.", "meta-convert help");
    }

    private static async Task<int> RunSchemaToRawDataVaultAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintSchemaToRawDataVaultHelp();
            return 0;
        }

        var parse = ParseSchemaToRawDataVaultArgs(args, startIndex: 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, "meta-convert schema-to-raw-datavault --help");
        }

        var sourceWorkspacePath = Path.GetFullPath(parse.SourceWorkspacePath);
        var targetWorkspacePath = Path.GetFullPath(parse.NewWorkspacePath);
        if (Directory.Exists(targetWorkspacePath) && Directory.EnumerateFileSystemEntries(targetWorkspacePath).Any())
        {
            return Fail($"target directory '{targetWorkspacePath}' must be empty.", "choose a new folder or empty the target directory and retry.", 4);
        }

        Directory.CreateDirectory(targetWorkspacePath);

        RawDataVaultFromMetaSchemaService.RawDataVaultFromMetaSchemaResult result;
        try
        {
            var sourceModel = await MetaSchemaInstance.LoadFromWorkspaceAsync(
                sourceWorkspacePath,
                searchUpward: false).ConfigureAwait(false);

            result = new RawDataVaultFromMetaSchemaService().MaterializeWithReport(
                sourceModel,
                parse.IgnoreFieldNames,
                parse.IgnoreFieldSuffixes,
                parse.IncludeViews);

            await MetaRawDataVaultInstance.SaveToWorkspaceAsync(
                result.Model,
                targetWorkspacePath).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Fail(
                "schema-to-raw-datavault conversion failed.",
                "check source workspace, output path, and options, then retry.",
                4,
                new[]
                {
                    $"  SourceWorkspace: {sourceWorkspacePath}",
                    $"  TargetWorkspace: {targetWorkspacePath}",
                    $"  {ex.Message}",
                });
        }

        Presenter.WriteOk($"Created {Path.GetFileName(targetWorkspacePath)}");
        if (parse.Verbose)
        {
            RenderSummary(result.Report.Summary);
        }

        return 0;
    }

    private static (bool Ok, string SourceWorkspacePath, string NewWorkspacePath, List<string> IgnoreFieldNames, List<string> IgnoreFieldSuffixes, bool IncludeViews, bool Verbose, string ErrorMessage) ParseSchemaToRawDataVaultArgs(
        string[] args,
        int startIndex)
    {
        var sourceWorkspacePath = string.Empty;
        var newWorkspacePath = string.Empty;
        var ignoreFieldNames = new List<string>();
        var ignoreFieldSuffixes = new List<string>();
        var includeViews = false;
        var verbose = false;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--source-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --source-workspace.");
                if (!string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "--source-workspace can only be provided once.");
                sourceWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "--new-workspace can only be provided once.");
                newWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--ignore-field-name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --ignore-field-name.");
                ignoreFieldNames.Add(args[++i]);
                continue;
            }

            if (string.Equals(arg, "--ignore-field-suffix", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --ignore-field-suffix.");
                ignoreFieldSuffixes.Add(args[++i]);
                continue;
            }

            if (string.Equals(arg, "--include-views", StringComparison.OrdinalIgnoreCase))
            {
                includeViews = true;
                continue;
            }

            if (string.Equals(arg, "--verbose", StringComparison.OrdinalIgnoreCase))
            {
                verbose = true;
                continue;
            }

            return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing required option --source-workspace <path>.");
        if (string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing required option --new-workspace <path>.");

        return (true, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, string.Empty);
    }

    private static void RenderSummary(RawDataVaultFromMetaSchemaSummary summary)
    {
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteKeyValueBlock("Summary", new[]
        {
            ("Source systems", summary.SourceSystemCount.ToString()),
            ("Source schemas", summary.SourceSchemaCount.ToString()),
            ("Source tables", summary.SourceTableCount.ToString()),
            ("Source relationships", summary.SourceRelationshipCount.ToString()),
            ("Raw hubs", summary.RawHubCount.ToString()),
            ("Raw hub key parts", summary.RawHubKeyPartCount.ToString()),
            ("Raw links", summary.RawLinkCount.ToString()),
            ("Raw hub satellites", summary.RawHubSatelliteCount.ToString()),
            ("Raw hub satellite attributes", summary.RawHubSatelliteAttributeCount.ToString()),
            ("Ignored field names", FormatSummaryList(summary.IgnoredFieldNames)),
            ("Ignored field suffixes", FormatSummaryList(summary.IgnoredFieldSuffixes)),
            ("Included views", summary.IncludeViews ? "yes" : "no"),
        });
    }

    private static string FormatSummaryList(IEnumerable<string> values)
    {
        var materialized = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ThenBy(value => value, StringComparer.Ordinal)
            .ToList();

        return materialized.Count == 0
            ? "(none)"
            : string.Join(", ", materialized);
    }

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        Presenter.WriteUsage("meta-convert <command> [options]");
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog(
            "Commands:",
            new[]
            {
                ("help", "Show this help."),
                ("schema-to-raw-datavault", "Convert MetaSchema workspace to MetaRawDataVault workspace.")
            });
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteNext("meta-convert schema-to-raw-datavault --help");
    }

    private static void PrintSchemaToRawDataVaultHelp()
    {
        Presenter.WriteInfo("Command: schema-to-raw-datavault");
        Presenter.WriteUsage("meta-convert schema-to-raw-datavault --source-workspace <path> --new-workspace <path> [--ignore-field-name <name>]... [--ignore-field-suffix <suffix>]... [--include-views] [--verbose]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads MetaSchema from --source-workspace and saves MetaRawDataVault at --new-workspace.");
        Presenter.WriteInfo("  Uses typed MetaSchema and MetaRawDataVault instance/tooling libraries.");
        Presenter.WriteInfo("  Does not use generic workspace model loading.");
    }

    private static int Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null)
        {
            renderedDetails.AddRange(details);
        }

        renderedDetails.Add($"Next: {next}");
        Presenter.WriteFailure(message, renderedDetails);
        return exitCode;
    }
}
