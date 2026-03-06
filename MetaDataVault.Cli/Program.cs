using System.Linq;
using Meta.Core.Domain;
using Meta.Core.Presentation;
using Meta.Core.Services;
using MetaDataVault.Core;

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

        if (string.Equals(args[0], "init", StringComparison.OrdinalIgnoreCase))
        {
            return await RunInitAsync(args).ConfigureAwait(false);
        }

        if (string.Equals(args[0], "from-metaschema", StringComparison.OrdinalIgnoreCase))
        {
            return await RunFromMetaSchemaAsync(args).ConfigureAwait(false);
        }

        return Fail($"unknown command '{args[0]}'.", "meta-datavault help");
    }

    private static async Task<int> RunInitAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintInitHelp();
            return 0;
        }

        var parseResult = ParseNewWorkspaceOnly(args, 1);
        if (!parseResult.Ok)
        {
            return Fail(parseResult.ErrorMessage, "meta-datavault init --help");
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
                "fix the sanctioned model and retry init.",
                4,
                validation.Issues
                    .Where(item => item.Severity == IssueSeverity.Error)
                    .Select(item => $"  - {item.Code}: {item.Message}"));
        }

        await new WorkspaceService().SaveAsync(workspace).ConfigureAwait(false);
        Presenter.WriteOk(
            "metarawdatavault workspace created",
            ("Path", workspacePath),
            ("Model", workspace.Model.Name));
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
            return Fail(parse.ErrorMessage, "meta-datavault from-metaschema --help");
        }

        var sourceWorkspacePath = Path.GetFullPath(parse.SourceWorkspacePath);
        var businessKeyWorkspacePath = Path.GetFullPath(parse.BusinessKeyWorkspacePath);
        var implementationWorkspacePath = Path.GetFullPath(parse.ImplementationWorkspacePath);
        var newWorkspacePath = Path.GetFullPath(parse.NewWorkspacePath);
        if (Directory.Exists(newWorkspacePath) && Directory.EnumerateFileSystemEntries(newWorkspacePath).Any())
        {
            return Fail($"target directory '{newWorkspacePath}' must be empty.", "choose a new folder or empty the target directory and retry.", 4);
        }

        Workspace sourceWorkspace;
        Workspace businessKeyWorkspace;
        Workspace implementationWorkspace;
        try
        {
            var workspaceService = new WorkspaceService();
            sourceWorkspace = await workspaceService.LoadAsync(sourceWorkspacePath, searchUpward: false).ConfigureAwait(false);
            businessKeyWorkspace = await workspaceService.LoadAsync(businessKeyWorkspacePath, searchUpward: false).ConfigureAwait(false);
            implementationWorkspace = await workspaceService.LoadAsync(implementationWorkspacePath, searchUpward: false).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Fail(
                "could not load sanctioned input workspaces.",
                "check the MetaSchema, MetaBusinessKey, and MetaDataVaultImplementation workspace paths and retry.",
                4,
                new[] { $"  - {ex.Message}" });
        }

        Workspace rawDataVaultWorkspace;
        try
        {
            rawDataVaultWorkspace = new MetaSchemaToRawDataVaultConverter().Convert(
                sourceWorkspace,
                businessKeyWorkspace,
                implementationWorkspace,
                newWorkspacePath);
        }
        catch (Exception ex)
        {
            return Fail(
                "could not materialize raw datavault from sanctioned inputs.",
                "check the MetaSchema, MetaBusinessKey, and MetaDataVaultImplementation workspaces and retry.",
                4,
                new[] { $"  - {ex.Message}" });
        }

        var validation = new ValidationService().Validate(rawDataVaultWorkspace);
        if (validation.HasErrors)
        {
            return Fail(
                "generated metarawdatavault workspace is invalid.",
                "inspect the generated workspace and retry.",
                4,
                validation.Issues
                    .Where(item => item.Severity == IssueSeverity.Error)
                    .Select(item => $"  - {item.Code}: {item.Message}"));
        }

        Directory.CreateDirectory(newWorkspacePath);
        await new WorkspaceService().SaveAsync(rawDataVaultWorkspace).ConfigureAwait(false);

        Presenter.WriteOk(
            "raw datavault materialized from sanctioned inputs",
            ("MetaSchema Workspace", sourceWorkspacePath),
            ("MetaBusinessKey Workspace", businessKeyWorkspacePath),
            ("MetaDataVaultImplementation Workspace", implementationWorkspacePath),
            ("Path", newWorkspacePath),
            ("Model", rawDataVaultWorkspace.Model.Name),
            ("SourceTables", rawDataVaultWorkspace.Instance.GetOrCreateEntityRecords("SourceTable").Count.ToString()),
            ("RawHubs", rawDataVaultWorkspace.Instance.GetOrCreateEntityRecords("RawHub").Count.ToString()),
            ("RawLinks", rawDataVaultWorkspace.Instance.GetOrCreateEntityRecords("RawLink").Count.ToString()),
            ("RawLinkEnds", rawDataVaultWorkspace.Instance.GetOrCreateEntityRecords("RawLinkEnd").Count.ToString()),
            ("RawHubSatellites", rawDataVaultWorkspace.Instance.GetOrCreateEntityRecords("RawHubSatellite").Count.ToString()),
            ("RawHubSatelliteAttributes", rawDataVaultWorkspace.Instance.GetOrCreateEntityRecords("RawHubSatelliteAttribute").Count.ToString()),
            ("RawLinkSatellites", rawDataVaultWorkspace.Instance.GetOrCreateEntityRecords("RawLinkSatellite").Count.ToString()),
            ("RawLinkSatelliteAttributes", rawDataVaultWorkspace.Instance.GetOrCreateEntityRecords("RawLinkSatelliteAttribute").Count.ToString()));
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

    private static (bool Ok, string SourceWorkspacePath, string BusinessKeyWorkspacePath, string ImplementationWorkspacePath, string NewWorkspacePath, string ErrorMessage) ParseFromMetaSchemaArgs(string[] args, int startIndex)
    {
        var sourceWorkspacePath = string.Empty;
        var businessKeyWorkspacePath = string.Empty;
        var implementationWorkspacePath = string.Empty;
        var newWorkspacePath = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--source-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, "missing value for --source-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(sourceWorkspacePath))
                {
                    return (false, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, "--source-workspace can only be provided once.");
                }

                sourceWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--business-key-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, "missing value for --business-key-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(businessKeyWorkspacePath))
                {
                    return (false, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, "--business-key-workspace can only be provided once.");
                }

                businessKeyWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--implementation-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, "missing value for --implementation-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(implementationWorkspacePath))
                {
                    return (false, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, "--implementation-workspace can only be provided once.");
                }

                implementationWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, "missing value for --new-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(newWorkspacePath))
                {
                    return (false, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, "--new-workspace can only be provided once.");
                }

                newWorkspacePath = args[++i];
                continue;
            }

            return (false, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(sourceWorkspacePath))
        {
            return (false, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, "missing required option --source-workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(businessKeyWorkspacePath))
        {
            return (false, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, "missing required option --business-key-workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(implementationWorkspacePath))
        {
            return (false, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, "missing required option --implementation-workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(newWorkspacePath))
        {
            return (false, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, "missing required option --new-workspace <path>.");
        }

        return (true, sourceWorkspacePath, businessKeyWorkspacePath, implementationWorkspacePath, newWorkspacePath, string.Empty);
    }

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        Presenter.WriteUsage("meta-datavault <command> [options]");
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog(
            "Commands:",
            new[]
            {
                ("help", "Show this help."),
                ("init", "Create an empty MetaRawDataVault workspace."),
                ("from-metaschema", "Materialize a raw datavault from MetaSchema, MetaBusinessKey, and MetaDataVaultImplementation workspaces.")
            });
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteNext("meta-datavault from-metaschema --help");
    }

    private static void PrintInitHelp()
    {
        Presenter.WriteInfo("Command: init");
        Presenter.WriteUsage("meta-datavault init --new-workspace <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Creates a new workspace with the sanctioned MetaRawDataVault model (raw vault only).");
    }

    private static void PrintFromMetaSchemaHelp()
    {
        Presenter.WriteInfo("Command: from-metaschema");
        Presenter.WriteUsage("meta-datavault from-metaschema --source-workspace <path> --business-key-workspace <path> --implementation-workspace <path> --new-workspace <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Loads sanctioned MetaSchema, MetaBusinessKey, and MetaDataVaultImplementation workspaces.");
        Presenter.WriteInfo("  Heuristic business-key inference was removed. Raw datavault materialization now requires explicit sanctioned inputs and weave bindings.");
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
