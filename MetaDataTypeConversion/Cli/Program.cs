using System.Linq;
using Meta.Core.Domain;
using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using Meta.Core.Services;
using MetaDataTypeConversion.Core;

internal static class Program
{
    private static readonly ConsolePresenter Presenter = new();
    private static readonly IReadOnlyList<CliCommandRoute> CommandRoutes = BuildCommandRoutes();
    private static readonly IReadOnlyDictionary<string, CliCommandRoute> CommandRoutesByName = CommandRoutes
        .ToDictionary(route => route.Definition.Name, StringComparer.OrdinalIgnoreCase);
    private static readonly CliAppDefinition Cli = new(
        "meta-data-type-conversion",
        new[]
        {
            "meta-data-type-conversion [--new-workspace <path> | <command> [options]]"
        },
        CommandRoutes.Select(route => route.Definition).ToArray(),
        Next: "meta-data-type-conversion --new-workspace --help");

    internal static CliAppDefinition CreateAppDefinition() => Cli;

    private static IReadOnlyList<CliCommandRoute> BuildCommandRoutes() =>
        new[]
        {
            new CliCommandRoute(
                new CliCommandDefinition(
                    "help",
                    "Show this help.",
                    new[] { "meta-data-type-conversion help" }),
                _ =>
                {
                    PrintHelp();
                    return Task.FromResult(0);
                }),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "--new-workspace",
                    "Create a new MetaDataTypeConversion workspace.",
                    new[] { "meta-data-type-conversion --new-workspace <path>" },
                    new[]
                    {
                        new CliOptionDefinition("--new-workspace <path>", "Required. Directory where the sanctioned workspace will be created.")
                    },
                    new[]
                    {
                        "Creates a new workspace with the MetaDataTypeConversion model and validates it."
                    }),
                RunNewWorkspaceAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "check",
                    "Validate sanctioned type mappings.",
                    new[] { "meta-data-type-conversion check --workspace <path>" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaDataTypeConversion workspace to validate.")
                    },
                    new[]
                    {
                        "Validates that each source data type maps deterministically per target data type system and that every mapping references a real ConversionImplementation."
                    }),
                RunCheckAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "resolve",
                    "Resolve one source data type id through the sanctioned mappings.",
                    new[] { "meta-data-type-conversion resolve --workspace <path> --source-data-type <id> [--target-data-type-system <name>]" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaDataTypeConversion workspace to query."),
                        new CliOptionDefinition("--source-data-type <id>", "Required. Source data type id to resolve."),
                        new CliOptionDefinition("--target-data-type-system <name>", "Optional target system when the source type has mappings to several target systems.")
                    },
                    new[]
                    {
                        "Resolves one source data type id to its target data type id and conversion implementation.",
                        "Use --target-data-type-system when one source type has mappings to several target systems."
                    }),
                RunResolveAsync)
        };

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

        return Fail($"unknown command '{args[0]}'.", $"{Cli.Name} help");
    }

    private static async Task<int> RunNewWorkspaceAsync(string[] args)
    {
        if (args.Length > 1 && IsHelpToken(args[1]))
        {
            PrintCommandHelp("--new-workspace");
            return 0;
        }

        var parseResult = ParseNewWorkspaceOnly(args, startIndex: 0);
        if (!parseResult.Ok)
        {
            return Fail(parseResult.ErrorMessage, HelpCommand("--new-workspace"));
        }

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parseResult.NewWorkspacePath);
        if (!targetValidation.Ok)
        {
            return Fail(targetValidation.ErrorMessage, "choose a new folder or empty the target directory and retry.", 4, targetValidation.Details);
        }

        var workspacePath = targetValidation.FullPath;
        Directory.CreateDirectory(workspacePath);

        var workspace = MetaDataTypeConversionWorkspaces.CreateMetaDataTypeConversionWorkspace(workspacePath);
        var validation = new ValidationService().Validate(workspace);
        if (validation.HasErrors)
        {
            return Fail(
                "MetaDataTypeConversion workspace is invalid.",
                "fix the sanctioned model and retry workspace creation.",
                4,
                validation.Issues
                    .Where(item => item.Severity == IssueSeverity.Error)
                    .Select(item => $"  - {item.Code}: {item.Message}"));
        }

        await new WorkspaceService().SaveAsync(workspace).ConfigureAwait(false);

        Presenter.WriteOk(
            "MetaDataTypeConversion workspace created",
            ("Path", workspacePath),
            ("Model", workspace.Model.Name),
            ("ConversionImplementations", workspace.Instance.GetOrCreateEntityRecords("ConversionImplementation").Count.ToString()),
            ("DataTypeMappings", workspace.Instance.GetOrCreateEntityRecords("DataTypeMapping").Count.ToString()));
        return 0;
    }

    private static async Task<int> RunCheckAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintCommandHelp("check");
            return 0;
        }

        var parseResult = ParseWorkspaceOnly(args, startIndex: 1);
        if (!parseResult.Ok)
        {
            return Fail(parseResult.ErrorMessage, HelpCommand("check"));
        }

        Workspace workspace;
        try
        {
            workspace = await new WorkspaceService().LoadAsync(Path.GetFullPath(parseResult.WorkspacePath), searchUpward: false).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Cannot check data-type conversions.",
                HelpCommand("check"),
                4,
                [$"  {ex.Message}"]);
        }

        var result = new MetaDataTypeConversionService().Check(workspace);
        if (result.HasErrors)
        {
            return Fail("Cannot check data-type conversions.", "fix the sanctioned mappings and rerun check.", 2, result.Errors.Select(error => $"  - {error}"));
        }

        Presenter.WriteOk(
            "MetaDataTypeConversion check",
            ("ConversionImplementations", result.ImplementationCount.ToString()),
            ("DataTypeMappings", result.MappingCount.ToString()),
            ("Errors", "0"));
        return 0;
    }

    private static async Task<int> RunResolveAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintCommandHelp("resolve");
            return 0;
        }

        var parseResult = ParseResolveArgs(args, startIndex: 1);
        if (!parseResult.Ok)
        {
            return Fail(parseResult.ErrorMessage, HelpCommand("resolve"));
        }

        Workspace workspace;
        try
        {
            workspace = await new WorkspaceService().LoadAsync(Path.GetFullPath(parseResult.WorkspacePath), searchUpward: false).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            return Fail(
                "Cannot resolve data-type conversion.",
                HelpCommand("resolve"),
                4,
                [$"  {ex.Message}"]);
        }

        try
        {
            var resolution = string.IsNullOrWhiteSpace(parseResult.TargetDataTypeSystemName)
                ? new MetaDataTypeConversionService().Resolve(workspace, parseResult.SourceDataTypeId)
                : new MetaDataTypeConversionService().Resolve(workspace, parseResult.SourceDataTypeId, parseResult.TargetDataTypeSystemName);
            var details = new List<(string Key, string Value)>
            {
                ("SourceDataTypeId", resolution.SourceDataTypeId),
                ("TargetDataTypeId", resolution.TargetDataTypeId),
                ("TargetDataTypeSystem", resolution.TargetDataTypeSystemName),
                ("ConversionImplementation", resolution.ConversionImplementationName)
            };

            if (!string.IsNullOrWhiteSpace(resolution.Notes))
            {
                details.Add(("Notes", resolution.Notes));
            }

            Presenter.WriteOk("MetaDataTypeConversion resolve", details.ToArray());
            return 0;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            return Fail(
                "Cannot resolve data-type conversion.",
                HelpCommand("resolve"),
                4,
                [$"  {ex.Message}"]);
        }
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

    private static (bool Ok, string WorkspacePath, string ErrorMessage) ParseWorkspaceOnly(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (!string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                return (false, workspacePath, $"unknown option '{arg}'.");
            }

            if (i + 1 >= args.Length)
            {
                return (false, workspacePath, "missing value for --workspace.");
            }

            if (!string.IsNullOrWhiteSpace(workspacePath))
            {
                return (false, workspacePath, "--workspace can only be provided once.");
            }

            workspacePath = args[++i];
        }

        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            return (false, string.Empty, "missing required option --workspace <path>.");
        }

        return (true, workspacePath, string.Empty);
    }

    private static (bool Ok, string WorkspacePath, string SourceDataTypeId, string TargetDataTypeSystemName, string ErrorMessage) ParseResolveArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var sourceDataTypeId = string.Empty;
        var targetDataTypeSystemName = string.Empty;
        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (i + 1 >= args.Length)
            {
                return (false, workspacePath, sourceDataTypeId, targetDataTypeSystemName, $"missing value for {arg}.");
            }

            switch (arg.ToLowerInvariant())
            {
                case "--workspace":
                    if (!string.IsNullOrWhiteSpace(workspacePath))
                    {
                        return (false, workspacePath, sourceDataTypeId, targetDataTypeSystemName, "--workspace can only be provided once.");
                    }
                    workspacePath = args[++i];
                    break;
                case "--source-data-type":
                    if (!string.IsNullOrWhiteSpace(sourceDataTypeId))
                    {
                        return (false, workspacePath, sourceDataTypeId, targetDataTypeSystemName, "--source-data-type can only be provided once.");
                    }
                    sourceDataTypeId = args[++i];
                    break;
                case "--target-data-type-system":
                    if (!string.IsNullOrWhiteSpace(targetDataTypeSystemName))
                    {
                        return (false, workspacePath, sourceDataTypeId, targetDataTypeSystemName, "--target-data-type-system can only be provided once.");
                    }
                    targetDataTypeSystemName = args[++i];
                    break;
                default:
                    return (false, workspacePath, sourceDataTypeId, targetDataTypeSystemName, $"unknown option '{arg}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            return (false, string.Empty, sourceDataTypeId, targetDataTypeSystemName, "missing required option --workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(sourceDataTypeId))
        {
            return (false, workspacePath, string.Empty, targetDataTypeSystemName, "missing required option --source-data-type <id>.");
        }

        return (true, workspacePath, sourceDataTypeId, targetDataTypeSystemName, string.Empty);
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
