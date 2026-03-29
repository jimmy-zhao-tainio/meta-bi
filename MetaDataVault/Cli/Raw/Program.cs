using System.Linq;
using Meta.Core.Domain;
using Meta.Core.Presentation;
using Meta.Core.Services;
using MetaDataVault.Core;
using MetaDataVault.FromMetaSchema;

internal static partial class Program
{
    private static readonly ConsolePresenter Presenter = new();

    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return 0;
        }

        if (string.Equals(args[0], "--new-workspace", StringComparison.OrdinalIgnoreCase))
        {
            return await RunNewWorkspaceAsync(args).ConfigureAwait(false);
        }

        if (string.Equals(args[0], "from-metaschema", StringComparison.OrdinalIgnoreCase))
        {
            return await RunFromMetaSchemaAsync(args).ConfigureAwait(false);
        }

        if (TryGetAddCommand(args[0], out var addCommand) && addCommand != null)
        {
            return await RunAddCommandAsync(addCommand, args).ConfigureAwait(false);
        }

        if (string.Equals(args[0], "generate-metasql", StringComparison.OrdinalIgnoreCase))
        {
            return await RunGenerateMetaSqlAsync(args).ConfigureAwait(false);
        }

        return Fail($"unknown command '{args[0]}'.", "meta-datavault-raw help");
    }

    private static async Task<int> RunNewWorkspaceAsync(string[] args)
    {
        var parseResult = ParseNewWorkspaceOnly(args, 0);
        if (!parseResult.Ok)
        {
            return Fail(parseResult.ErrorMessage, "meta-datavault-raw --new-workspace <path>");
        }

        var workspacePath = Path.GetFullPath(parseResult.NewWorkspacePath);
        if (Directory.Exists(workspacePath) && Directory.EnumerateFileSystemEntries(workspacePath).Any())
        {
            return Fail($"target directory '{workspacePath}' must be empty.", "choose a new folder or empty the target directory and retry.", 4);
        }

        Directory.CreateDirectory(workspacePath);
        var workspace = MetaDataVaultWorkspaces.CreateEmptyMetaRawDataVaultWorkspace(workspacePath);
        var validation = new ValidationService().Validate(workspace);
        if (validation.HasErrors)
        {
            return Fail(
                "metarawdatavault workspace is invalid.",
                "fix the sanctioned model and retry workspace creation.",
                4,
                validation.Issues.Where(item => item.Severity == IssueSeverity.Error).Select(item => $"  - {item.Code}: {item.Message}"));
        }

        await new WorkspaceService().SaveAsync(workspace).ConfigureAwait(false);
        Presenter.WriteOk($"Created {Path.GetFileName(workspacePath)}");
        return 0;
    }

    private static async Task<int> RunFromMetaSchemaAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintFromMetaSchemaHelp();
            return 0;
        }

        var parse = ParseFromMetaSchemaArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, "meta-datavault-raw from-metaschema --help");
        }

        var sourceWorkspacePath = Path.GetFullPath(parse.SourceWorkspacePath);
        var newWorkspacePath = Path.GetFullPath(parse.NewWorkspacePath);

        RawDataVaultFromMetaSchemaService.RawDataVaultFromMetaSchemaResult conversionResult;
        try
        {
            conversionResult = await new RawDataVaultFromMetaSchemaService().MaterializeWithReportAsync(
                sourceWorkspacePath,
                newWorkspacePath,
                parse.IgnoreFieldNames,
                parse.IgnoreFieldSuffixes,
                parse.IncludeViews).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Fail(
                "could not materialize raw datavault from metaschema workspace.",
                "check the source workspace, output path, and options, then retry.",
                4,
                new[] { $"  - {ex.Message}" });
        }

        Presenter.WriteOk($"Materialized {Path.GetFileName(newWorkspacePath)} from MetaSchema");
        if (parse.IgnoreFieldNames.Count > 0)
        {
            Presenter.WriteInfo($"Ignored Field Names: {FormatSummaryList(parse.IgnoreFieldNames)}");
        }

        if (parse.IgnoreFieldSuffixes.Count > 0)
        {
            Presenter.WriteInfo($"Ignored Field Suffixes: {FormatSummaryList(parse.IgnoreFieldSuffixes)}");
        }

        if (parse.IncludeViews)
        {
            Presenter.WriteInfo("Included Views: yes");
        }

        if (parse.Verbose)
        {
            RenderFromMetaSchemaReport(conversionResult.Report);
        }

        return 0;
    }

    private static (bool Ok, string NewWorkspacePath, string ErrorMessage) ParseNewWorkspaceOnly(string[] args, int startIndex)
    {
        var newWorkspacePath = string.Empty;
        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (!string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                return (false, newWorkspacePath, $"unknown option '{arg}'.");
            }

            if (i + 1 >= args.Length)
            {
                return (false, newWorkspacePath, "missing value for --new-workspace.");
            }

            if (!string.IsNullOrWhiteSpace(newWorkspacePath))
            {
                return (false, newWorkspacePath, "--new-workspace can only be provided once.");
            }

            newWorkspacePath = args[++i];
        }

        if (string.IsNullOrWhiteSpace(newWorkspacePath))
        {
            return (false, string.Empty, "missing required option --new-workspace <path>.");
        }

        return (true, newWorkspacePath, string.Empty);
    }

    private static (bool Ok, string SourceWorkspacePath, string NewWorkspacePath, List<string> IgnoreFieldNames, List<string> IgnoreFieldSuffixes, bool IncludeViews, bool Verbose, string ErrorMessage) ParseFromMetaSchemaArgs(string[] args, int startIndex)
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

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
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

    private static void RenderFromMetaSchemaReport(RawDataVaultFromMetaSchemaReport report)
    {
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteKeyValueBlock("Summary", new[]
        {
            ("Source systems", report.Summary.SourceSystemCount.ToString()),
            ("Source schemas", report.Summary.SourceSchemaCount.ToString()),
            ("Source tables", report.Summary.SourceTableCount.ToString()),
            ("Source relationships", report.Summary.SourceRelationshipCount.ToString()),
            ("Raw hubs", report.Summary.RawHubCount.ToString()),
            ("Raw hub key parts", report.Summary.RawHubKeyPartCount.ToString()),
            ("Raw links", report.Summary.RawLinkCount.ToString()),
            ("Raw hub satellites", report.Summary.RawHubSatelliteCount.ToString()),
            ("Raw hub satellite attributes", report.Summary.RawHubSatelliteAttributeCount.ToString()),
            ("Ignored field names", FormatSummaryList(report.Summary.IgnoredFieldNames)),
            ("Ignored field suffixes", FormatSummaryList(report.Summary.IgnoredFieldSuffixes)),
            ("Included views", report.Summary.IncludeViews ? "yes" : "no"),
        });

        var materializedTables = report.Tables
            .Where(table => table.HubCreated)
            .ToList();
        if (materializedTables.Count > 0)
        {
            Presenter.WriteInfo(string.Empty);
            Presenter.WriteInfo("Hubs");
            foreach (var table in materializedTables)
            {
                Presenter.WriteInfo($"  {table.QualifiedTableName} -> {FormatSelectedKey(table.SelectedKey)}; satellite attributes: {table.SatelliteAttributeCount}");
            }
        }

        var skippedTables = report.Tables
            .Where(table => !table.HubCreated)
            .ToList();
        if (skippedTables.Count > 0)
        {
            Presenter.WriteInfo(string.Empty);
            Presenter.WriteInfo("Skipped Tables");
            foreach (var table in skippedTables)
            {
                Presenter.WriteInfo($"  {table.QualifiedTableName} -> {table.Reason ?? "no usable key"}");
            }
        }

        var materializedLinks = report.Relationships
            .Where(relationship => relationship.LinkCreated)
            .ToList();
        if (materializedLinks.Count > 0)
        {
            Presenter.WriteInfo(string.Empty);
            Presenter.WriteInfo("Links");
            foreach (var relationship in materializedLinks)
            {
                var suffix = relationship.NameWasDisambiguated ? " (disambiguated)" : string.Empty;
                Presenter.WriteInfo($"  {relationship.RawLinkName} -> {relationship.SourceTableName} -> {relationship.TargetTableName}{suffix}");
            }
        }

        var skippedRelationships = report.Relationships
            .Where(relationship => !relationship.LinkCreated)
            .ToList();
        if (skippedRelationships.Count > 0)
        {
            Presenter.WriteInfo(string.Empty);
            Presenter.WriteInfo("Skipped Relationships");
            foreach (var relationship in skippedRelationships)
            {
                Presenter.WriteInfo($"  {relationship.SourceTableName} -> {relationship.TargetTableName} -> {relationship.Reason ?? "not materialized"}");
            }
        }
    }

    private static string FormatSelectedKey(RawDataVaultFromMetaSchemaSelectedKeyReport? selectedKey)
    {
        if (selectedKey == null)
        {
            return "none";
        }

        var keyType = selectedKey.KeyType switch
        {
            "primary" => "primary key",
            "unique" => "unique key",
            _ => "key",
        };
        var fieldList = string.Join("`, `", selectedKey.FieldNames);
        return string.IsNullOrWhiteSpace(selectedKey.KeyName)
            ? $"{keyType} -> `{fieldList}`"
            : $"{keyType} `{selectedKey.KeyName}` -> `{fieldList}`";
    }

    private static int Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null) renderedDetails.AddRange(details);
        renderedDetails.Add($"Next: {next}");
        Presenter.WriteFailure(message, renderedDetails);
        return exitCode;
    }

    private static void PrintHelp()
    {
        Presenter.WriteUsage("meta-datavault-raw [--new-workspace <path> | <command> [options]]");
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog("Commands:", new[]
        {
            ("help", "Show this help."),
            ("--new-workspace", "Create an empty MetaRawDataVault workspace."),
            ("from-metaschema", "Materialize a raw datavault from a MetaSchema workspace."),
            ("generate-metasql", "Generate a current MetaSql workspace from the current raw datavault workspace."),
            ("add-*", "Add sanctioned MetaRawDataVault rows.")
        });
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog("Authoring commands:", GetAddCommandCatalog().ToList());
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteNext("meta-datavault-raw generate-metasql --help");
    }

    private static void PrintFromMetaSchemaHelp()
    {
        Presenter.WriteInfo("Command: from-metaschema");
        Presenter.WriteUsage("meta-datavault-raw from-metaschema --source-workspace <path> --new-workspace <path> [--ignore-field-name <name>]... [--ignore-field-suffix <suffix>]... [--include-views] [--verbose]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads a sanctioned MetaSchema workspace and materializes a MetaRawDataVault workspace.");
        Presenter.WriteInfo("  Hubs come from source-local primary or unique keys. Links come from MetaSchema table relationships.");
        Presenter.WriteInfo("  Views are excluded by default. Use --include-views to keep them in scope.");
        Presenter.WriteInfo("  Use --verbose to print a compact materialization summary, skipped decisions, and disambiguated link names.");
        Presenter.WriteInfo("  Key selection is schema-driven and agnostic to source field names.");
        Presenter.WriteInfo("  Use --ignore-field-name and/or --ignore-field-suffix only for explicit source-field exclusion.");
    }
}
