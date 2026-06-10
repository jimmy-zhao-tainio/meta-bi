using Meta.Core.Domain;
using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using Meta.Core.Services;
using MetaAnalytics.Core;

internal static partial class Program
{
    private const string AppName = "meta-analytics";

    private static readonly ConsolePresenter Presenter = new();
    private static readonly Lazy<IReadOnlyList<CliCommandRoute>> CommandRoutesLazy = new(BuildCommandRoutes);
    private static readonly Lazy<IReadOnlyDictionary<string, CliCommandRoute>> CommandRoutesByNameLazy = new(
        () => CommandRoutes.ToDictionary(route => route.Definition.Name, StringComparer.OrdinalIgnoreCase));
    private static readonly Lazy<CliAppDefinition> CliLazy = new(
        () => new CliAppDefinition(
            AppName,
            new[]
            {
                "meta-analytics [--new-workspace <path> | <command> [options]]"
            },
            CommandRoutes.Select(route => route.Definition).ToArray(),
            new[]
            {
                "MetaAnalytics owns common analytics concepts; target-specific scripts and deployment belong in MetaTabular or MetaMultiDimensional."
            },
            "meta-analytics add-model --help"));

    private static IReadOnlyList<CliCommandRoute> CommandRoutes => CommandRoutesLazy.Value;

    private static IReadOnlyDictionary<string, CliCommandRoute> CommandRoutesByName => CommandRoutesByNameLazy.Value;

    private static CliAppDefinition Cli => CliLazy.Value;

    internal static CliAppDefinition CreateAppDefinition() => Cli;

    private static IReadOnlyList<CliCommandRoute> BuildCommandRoutes()
    {
        var routes = new List<CliCommandRoute>
        {
            new(
                new CliCommandDefinition(
                    "help",
                    "Show this help.",
                    new[] { "meta-analytics help" }),
                _ =>
                {
                    PrintHelp();
                    return Task.FromResult(0);
                }),
            new(
                new CliCommandDefinition(
                    "--new-workspace",
                    "Create an empty MetaAnalytics workspace.",
                    new[] { "meta-analytics --new-workspace <path>" },
                    new[]
                    {
                        new CliOptionDefinition("--new-workspace <path>", "Required. Directory where the empty MetaAnalytics workspace will be created.")
                    }),
                RunNewWorkspaceAsync)
        };

        routes.AddRange(BuildAddCommandRoutes());
        return routes;
    }

    static async Task<int> Main(string[] args)
    {
        if (Meta.Core.Presentation.Cli.CliVersion.TryWriteVersion(Presenter, Cli.Name, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return 0;
        }

        if (CommandRoutesByName.TryGetValue(args[0], out var route))
        {
            return await route.ExecuteAsync(args).ConfigureAwait(false);
        }

        if (args[0].StartsWith("--", StringComparison.Ordinal))
        {
            return await RunNewWorkspaceAsync(args).ConfigureAwait(false);
        }

        return Fail($"unknown command '{args[0]}'.", $"{AppName} help");
    }

    private static async Task<int> RunNewWorkspaceAsync(string[] args)
    {
        if (args.Length > 1 && IsHelpToken(args[1]))
        {
            PrintCommandHelp("--new-workspace");
            return 0;
        }

        var parse = ParseNewWorkspaceOnly(args, 0);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("--new-workspace"));
        }

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parse.NewWorkspacePath);
        if (!targetValidation.Ok)
        {
            return Fail(
                targetValidation.ErrorMessage,
                "choose a new folder or empty the target directory and retry.",
                4,
                targetValidation.Details);
        }

        var workspacePath = targetValidation.FullPath;
        Directory.CreateDirectory(workspacePath);
        var workspace = MetaAnalyticsWorkspaces.CreateEmptyMetaAnalyticsWorkspace(workspacePath);
        var validation = new ValidationService().Validate(workspace);
        if (validation.HasErrors)
        {
            return Fail(
                "metaanalytics workspace is invalid.",
                "fix the sanctioned model and retry workspace creation.",
                4,
                validation.Issues
                    .Where(item => item.Severity == IssueSeverity.Error)
                    .Select(item => $"  - {item.Code}: {item.Message}"));
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

    private static void PrintHelp()
    {
        CliHelpRenderer.WriteAppHelp(Presenter, Cli);
    }

    private static void PrintCommandHelp(string commandName)
    {
        CliHelpRenderer.WriteCommandHelp(Presenter, Cli, Cli.GetCommand(commandName));
    }

    private static string HelpCommand(string commandName) => Cli.GetCommand(commandName).HelpCommand(Cli.Name);

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
