using Meta.Core.Domain;
using Meta.Core.Presentation;
using Meta.Core.Services;
using MetaType.Core;

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

        return FailUnknownCommand(args[0], "meta-type help");
    }

    private static async Task<int> RunInitAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintInitHelp();
            return 0;
        }

        var parseResult = ParseNewWorkspaceOnly(args, startIndex: 1);
        if (!parseResult.Ok)
        {
            return Fail(parseResult.ErrorMessage, "meta-type init --help");
        }

        var workspacePath = Path.GetFullPath(parseResult.NewWorkspacePath);
        if (Directory.Exists(workspacePath) && Directory.EnumerateFileSystemEntries(workspacePath).Any())
        {
            return Fail($"target directory '{workspacePath}' must be empty.", "choose a new folder or empty the target directory and retry.", 4);
        }

        Directory.CreateDirectory(workspacePath);

        var workspace = MetaTypeWorkspaces.CreateMetaTypeWorkspace(workspacePath);
        var validation = new ValidationService().Validate(workspace);
        if (validation.HasErrors)
        {
            return Fail(
                "metatype workspace is invalid.",
                "fix the sanctioned model and retry init.",
                4,
                validation.Issues
                    .Where(item => item.Severity == IssueSeverity.Error)
                    .Select(item => $"  - {item.Code}: {item.Message}"));
        }

        await new WorkspaceService().SaveAsync(workspace).ConfigureAwait(false);

        Presenter.WriteOk(
            "metatype workspace created",
            ("Path", workspacePath),
            ("Model", workspace.Model.Name),
            ("TypeSystems", workspace.Instance.GetOrCreateEntityRecords("TypeSystem").Count.ToString()),
            ("Types", workspace.Instance.GetOrCreateEntityRecords("Type").Count.ToString()),
            ("TypeSpecs", workspace.Instance.GetOrCreateEntityRecords("TypeSpec").Count.ToString()));
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

    private static void PrintHelp()
    {
        Presenter.WriteInfo("MetaType CLI");
        Presenter.WriteUsage("meta-type <command> [options]");
        Presenter.WriteCommandCatalog(
            "Commands",
            new[]
            {
                ("help", "Show this help."),
                ("init", "Create a new MetaType workspace.")
            });
        Presenter.WriteNext("meta-type init --help");
    }

    private static void PrintInitHelp()
    {
        Presenter.WriteInfo("Command: init");
        Presenter.WriteUsage("meta-type init --new-workspace <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Creates a new workspace with the MetaType model, sanctioned type instances, and validates it.");
    }

    private static int FailUnknownCommand(string command, string next)
    {
        Presenter.WriteFailure($"unknown command '{command}'.", new[] { $"Next: {next}" });
        return 1;
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
