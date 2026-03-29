using System.Linq;
using Meta.Core.Domain;
using Meta.Core.Presentation;
using Meta.Core.Services;
using MetaDataVault.Core;

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

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
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
            ("generate-metasql", "Generate a current MetaSql workspace from the current raw datavault workspace."),
            ("add-*", "Add sanctioned MetaRawDataVault rows.")
        });
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog("Authoring commands:", GetAddCommandCatalog().ToList());
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteNext("meta-datavault-raw generate-metasql --help");
    }
}
