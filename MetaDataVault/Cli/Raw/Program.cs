using System.Linq;
using Meta.Core.Domain;
using Meta.Core.Presentation;
using Meta.Core.Services;
using MetaDataVault.Core;
using MetaSchema.ToRawDataVault;

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
        var businessWorkspacePath = string.IsNullOrWhiteSpace(parse.BusinessWorkspacePath) ? string.Empty : Path.GetFullPath(parse.BusinessWorkspacePath);
        var implementationWorkspacePath = Path.GetFullPath(parse.ImplementationWorkspacePath);
        var newWorkspacePath = Path.GetFullPath(parse.NewWorkspacePath);
        if (Directory.Exists(newWorkspacePath) && Directory.EnumerateFileSystemEntries(newWorkspacePath).Any())
        {
            return Fail($"target directory '{newWorkspacePath}' must be empty.", "choose a new folder or empty the target directory and retry.", 4);
        }

        MetaSchema.MetaSchemaModel sourceModel;
        Workspace? businessWorkspace = null;
        RawDataVaultImplementationModel implementationModel;
        try
        {
            var workspaceService = new WorkspaceService();
            sourceModel = await MetaSchema.MetaSchemaModel.LoadFromXmlWorkspaceAsync(sourceWorkspacePath, searchUpward: false).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(businessWorkspacePath))
            {
                businessWorkspace = await workspaceService.LoadAsync(businessWorkspacePath, searchUpward: false).ConfigureAwait(false);
                EnsureWorkspaceModel(businessWorkspace, "MetaBusiness", nameof(parse.BusinessWorkspacePath));
            }
            implementationModel = await RawDataVaultImplementationLoaders.LoadImplementationAsync(implementationWorkspacePath).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Fail(
                "could not load sanctioned input workspaces.",
                "check the MetaSchema, MetaDataVaultImplementation, and any optional MetaBusiness workspace paths, then retry.",
                4,
                new[] { $"  - {ex.Message}" });
        }

        RawDataVaultBootstrapper.RawDataVaultBootstrapResult conversionResult;
        try
        {
            conversionResult = new RawDataVaultBootstrapper().BootstrapWithReport(
                sourceModel,
                newWorkspacePath,
                implementationModel,
                parse.IgnoreFieldNames,
                parse.IgnoreFieldSuffixes,
                parse.IncludeViews);
        }
        catch (Exception ex)
        {
            return Fail(
                "could not materialize raw datavault from sanctioned inputs.",
                "check the MetaSchema workspace contents and retry. MetaDataVaultImplementation is required and any optional MetaBusiness workspace must match its sanctioned model.",
                4,
                new[] { $"  - {ex.Message}" });
        }

        var rawDataVaultWorkspace = conversionResult.Workspace;
        var validation = new ValidationService().Validate(rawDataVaultWorkspace);
        if (validation.HasErrors)
        {
            return Fail(
                "generated metarawdatavault workspace is invalid.",
                "inspect the generated workspace and retry.",
                4,
                validation.Issues.Where(item => item.Severity == IssueSeverity.Error).Select(item => $"  - {item.Code}: {item.Message}"));
        }

        Directory.CreateDirectory(newWorkspacePath);
        await new WorkspaceService().SaveAsync(rawDataVaultWorkspace).ConfigureAwait(false);

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
            Presenter.WriteInfo(string.Empty);
            foreach (var line in conversionResult.MaterializationReport.Split(Environment.NewLine))
            {
                Presenter.WriteInfo(line);
            }
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

    private static (bool Ok, string SourceWorkspacePath, string BusinessWorkspacePath, string ImplementationWorkspacePath, string NewWorkspacePath, List<string> IgnoreFieldNames, List<string> IgnoreFieldSuffixes, bool IncludeViews, bool Verbose, string ErrorMessage) ParseFromMetaSchemaArgs(string[] args, int startIndex)
    {
        var sourceWorkspacePath = string.Empty;
        var businessWorkspacePath = string.Empty;
        var implementationWorkspacePath = string.Empty;
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
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --source-workspace.");
                if (!string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "--source-workspace can only be provided once.");
                sourceWorkspacePath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--business-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --business-workspace.");
                if (!string.IsNullOrWhiteSpace(businessWorkspacePath)) return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "--business-workspace can only be provided once.");
                businessWorkspacePath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--implementation-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --implementation-workspace.");
                if (!string.IsNullOrWhiteSpace(implementationWorkspacePath)) return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "--implementation-workspace can only be provided once.");
                implementationWorkspacePath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "--new-workspace can only be provided once.");
                newWorkspacePath = args[++i];
                continue;
            }
            if (string.Equals(arg, "--ignore-field-name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --ignore-field-name.");
                ignoreFieldNames.Add(args[++i]);
                continue;
            }
            if (string.Equals(arg, "--ignore-field-suffix", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --ignore-field-suffix.");
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

            return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing required option --source-workspace <path>.");
        if (string.IsNullOrWhiteSpace(implementationWorkspacePath)) return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing required option --implementation-workspace <path>.");
        if (string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing required option --new-workspace <path>.");

        return (true, sourceWorkspacePath, businessWorkspacePath, implementationWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, string.Empty);
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
        Presenter.WriteUsage("meta-datavault-raw from-metaschema --source-workspace <path> --implementation-workspace <path> --new-workspace <path> [--business-workspace <path>] [--ignore-field-name <name>]... [--ignore-field-suffix <suffix>]... [--include-views] [--verbose]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads a sanctioned MetaSchema workspace and materializes a MetaRawDataVault workspace.");
        Presenter.WriteInfo("  Hubs come from source-local primary or unique keys. Links come from MetaSchema table relationships.");
        Presenter.WriteInfo("  MetaDataVaultImplementation is required sanctioned context for the materialization contract.");
        Presenter.WriteInfo("  Views are excluded by default. Use --include-views to keep them in scope.");
        Presenter.WriteInfo("  Use --verbose to print table and relationship materialization decisions to the console.");
        Presenter.WriteInfo("  MetaBusiness remains optional and is only validated when supplied.");
        Presenter.WriteInfo("  Key selection is schema-driven and agnostic to source field names.");
        Presenter.WriteInfo("  Use --ignore-field-name and/or --ignore-field-suffix only for explicit source-field exclusion.");
    }

    private static void EnsureWorkspaceModel(Workspace workspace, string expectedModelName, string parameterName)
    {
        if (!string.Equals(workspace.Model.Name, expectedModelName, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"Expected sanctioned model '{expectedModelName}' but found '{workspace.Model.Name}'.",
                parameterName);
        }
    }
}
