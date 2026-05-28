using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaTransform.Binding;

internal static class Program
{
    private const string AppName = "meta-transform-binding";

    private static readonly ConsolePresenter Presenter = new();
    private static readonly IReadOnlyList<CliCommandRoute> CommandRoutes = BuildCommandRoutes();
    private static readonly IReadOnlyDictionary<string, CliCommandRoute> CommandRoutesByName = CommandRoutes
        .ToDictionary(route => route.Definition.Name, StringComparer.OrdinalIgnoreCase);
    private static readonly CliAppDefinition Cli = new(
        AppName,
        new[]
        {
            "meta-transform-binding <command> [options]"
        },
        CommandRoutes.Select(route => route.Definition).ToArray(),
        Next: "meta-transform-binding bind --help");

    private static IReadOnlyList<CliCommandRoute> BuildCommandRoutes() =>
        new[]
        {
            new CliCommandRoute(
                new CliCommandDefinition(
                    "help",
                    "Show this help.",
                    new[] { "meta-transform-binding help" }),
                _ =>
                {
                    PrintHelp();
                    return Task.FromResult(0);
                }),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "bind",
                    "Bind all transform scripts and validate against source/target schema contracts into a new workspace.",
                    new[]
                    {
                        "meta-transform-binding bind --transform-workspace <path> --source-schema <path> [--source-schema <path> ...] --target-schema <path> --execute-system <name> --new-workspace <path> [--execute-system-default-schema-name <schema>] [--ignore-target-columns <col[,col...]>] [--data-type-conversion-workspace <path>]"
                    },
                    new[]
                    {
                        new CliOptionDefinition("--transform-workspace <path>", "Required. MetaTransformScript workspace to bind."),
                        new CliOptionDefinition("--source-schema <path>", "Required. Repeatable source MetaSchema workspace."),
                        new CliOptionDefinition("--target-schema <path>", "Required. Target MetaSchema workspace."),
                        new CliOptionDefinition("--execute-system <name>", "Required. Execution context for one/two-part source identifiers."),
                        new CliOptionDefinition("--new-workspace <path>", "Required. Directory where the binding workspace will be created."),
                        new CliOptionDefinition("--execute-system-default-schema-name <schema>", "Required when any one-part source identifier exists."),
                        new CliOptionDefinition("--ignore-target-columns <col[,col...]>", "Optional comma-separated target columns to exclude from target conformance checks."),
                        new CliOptionDefinition("--data-type-conversion-workspace <path>", "Optional sanctioned conversion policy workspace. Omitted uses built-in defaults.")
                    },
                    new[]
                    {
                        "bind is atomic: it binds and validates in one run.",
                        "If binding or validation fails, no binding workspace is created.",
                        "bind processes all transform scripts in the transform workspace.",
                        "Target SQL identifier is read from ScriptObjectView.TargetSqlIdentifier.",
                        "Source schema workspaces are repeatable; target schema workspace is single.",
                        "Every schema workspace must contain exactly one system.",
                        "--execute-system-default-schema-name is required when any one-part source identifier exists.",
                        "--ignore-target-columns excludes named non-identity target columns from target conformance checks.",
                        "Ignored names must exist on each target table or bind fails explicitly."
                    },
                    new[]
                    {
                        "meta-transform-binding bind --transform-workspace .\\TransformWS --source-schema .\\SourceSchemaWS --target-schema .\\TargetSchemaWS --execute-system SalesDb --new-workspace .\\BindingWS",
                        "meta-transform-binding bind --transform-workspace .\\TransformWS --source-schema .\\SalesSchemaWS --source-schema .\\ReferenceSchemaWS --target-schema .\\WarehouseSchemaWS --execute-system WarehouseDb --execute-system-default-schema-name dbo --new-workspace .\\BindingWS --ignore-target-columns LoadUtc,RunId"
                    }),
                args => RunBindAsync(args, startIndex: 1))
        };

    static Task<int> Main(string[] args)
    {
        if (Meta.Core.Presentation.Cli.CliVersion.TryWriteVersion(Presenter, Cli.Name, args, out var versionExitCode))
        {
            return Task.FromResult(versionExitCode);
        }

        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return Task.FromResult(0);
        }

        if (CommandRoutesByName.TryGetValue(args[0], out var route))
        {
            return route.ExecuteAsync(args);
        }

        return Task.FromResult(Fail($"unknown command '{args[0]}'.", $"{AppName} help"));
    }

    private static Task<int> RunBindAsync(string[] args, int startIndex)
    {
        if (args.Length == startIndex || (args.Length > startIndex && IsHelpToken(args[startIndex])))
        {
            PrintBindHelp();
            return Task.FromResult(0);
        }

        var parse = ParseBindArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Task.FromResult(Fail(parse.ErrorMessage, HelpCommand("bind")));
        }

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parse.NewWorkspacePath);
        if (!targetValidation.Ok)
        {
            return Task.FromResult(Fail(
                targetValidation.ErrorMessage,
                "choose a new folder or empty the target directory and retry.",
                4,
                targetValidation.Details));
        }

        try
        {
            var options = TransformBindingValidationOptions.Create(
                parse.IgnoredTargetColumns,
                parse.ExecuteSystemName,
                parse.ExecuteSystemDefaultSchemaName);

            using (var activity = CliActivityLine.Start("Binding"))
            {
                new TransformBindingWorkspaceService().BindValidatedToWorkspace(
                    parse.TransformWorkspacePath,
                    parse.SourceSchemaWorkspacePaths,
                    parse.TargetSchemaWorkspacePath,
                    parse.ExecuteSystemName,
                    parse.ExecuteSystemDefaultSchemaName,
                    targetValidation.FullPath,
                    validationOptions: options,
                    dataTypeConversionWorkspacePath: parse.DataTypeConversionWorkspacePath);

                activity.Succeed();
            }

            return Task.FromResult(0);
        }
        catch (TransformBindingValidationException ex)
        {
            return Task.FromResult(Fail(
                "Cannot validate binding.",
                "fix the schema or transform contract mismatch and retry.",
                5,
                new[]
                {
                    $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                    $"  SourceSchemas: {string.Join(", ", parse.SourceSchemaWorkspacePaths.Select(Path.GetFullPath))}",
                    $"  TargetSchema: {Path.GetFullPath(parse.TargetSchemaWorkspacePath)}",
                    $"  DataTypeConversionWorkspace: {(string.IsNullOrWhiteSpace(parse.DataTypeConversionWorkspacePath) ? "<default>" : Path.GetFullPath(parse.DataTypeConversionWorkspacePath))}",
                    $"  ExecuteSystem: {parse.ExecuteSystemName}",
                    $"  ExecuteSystemDefaultSchemaName: {(string.IsNullOrWhiteSpace(parse.ExecuteSystemDefaultSchemaName) ? "<none>" : parse.ExecuteSystemDefaultSchemaName)}",
                    $"  Code: {ex.Code}",
                    $"  {ex.Message}"
                }));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Fail(
                "Cannot create binding workspace.",
                "check transform/source-schema/target-schema inputs and retry.",
                4,
                new[]
                {
                    $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                    $"  SourceSchemas: {string.Join(", ", parse.SourceSchemaWorkspacePaths.Select(Path.GetFullPath))}",
                    $"  TargetSchema: {Path.GetFullPath(parse.TargetSchemaWorkspacePath)}",
                    $"  DataTypeConversionWorkspace: {(string.IsNullOrWhiteSpace(parse.DataTypeConversionWorkspacePath) ? "<default>" : Path.GetFullPath(parse.DataTypeConversionWorkspacePath))}",
                    $"  ExecuteSystem: {parse.ExecuteSystemName}",
                    $"  ExecuteSystemDefaultSchemaName: {(string.IsNullOrWhiteSpace(parse.ExecuteSystemDefaultSchemaName) ? "<none>" : parse.ExecuteSystemDefaultSchemaName)}",
                    $"  BindingWorkspace: {targetValidation.FullPath}",
                    $"  {ex.Message}"
                }));
        }
    }

    private static (
        bool Ok,
        string TransformWorkspacePath,
        string[] SourceSchemaWorkspacePaths,
        string TargetSchemaWorkspacePath,
        string ExecuteSystemName,
        string ExecuteSystemDefaultSchemaName,
        string NewWorkspacePath,
        string[] IgnoredTargetColumns,
        string DataTypeConversionWorkspacePath,
        string ErrorMessage) ParseBindArgs(
        string[] args,
        int startIndex)
    {
        var transformWorkspacePath = string.Empty;
        var sourceSchemaWorkspacePaths = new List<string>();
        var targetSchemaWorkspacePath = string.Empty;
        var executeSystemName = string.Empty;
        var executeSystemDefaultSchemaName = string.Empty;
        var newWorkspacePath = string.Empty;
        var ignoredTargetColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var dataTypeConversionWorkspacePath = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--transform-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "missing value for --transform-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(transformWorkspacePath))
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "--transform-workspace can only be provided once.");
                }

                transformWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--source-schema", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "missing value for --source-schema.");
                }

                sourceSchemaWorkspacePaths.Add(args[++i]);
                continue;
            }

            if (string.Equals(arg, "--target-schema", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "missing value for --target-schema.");
                }

                if (!string.IsNullOrWhiteSpace(targetSchemaWorkspacePath))
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "--target-schema can only be provided once.");
                }

                targetSchemaWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--execute-system", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "missing value for --execute-system.");
                }

                if (!string.IsNullOrWhiteSpace(executeSystemName))
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "--execute-system can only be provided once.");
                }

                executeSystemName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--execute-system-default-schema-name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "missing value for --execute-system-default-schema-name.");
                }

                if (!string.IsNullOrWhiteSpace(executeSystemDefaultSchemaName))
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "--execute-system-default-schema-name can only be provided once.");
                }

                executeSystemDefaultSchemaName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "missing value for --new-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(newWorkspacePath))
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "--new-workspace can only be provided once.");
                }

                newWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--ignore-target-columns", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "missing value for --ignore-target-columns.");
                }

                var raw = args[++i];
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "value for --ignore-target-columns cannot be blank.");
                }

                foreach (var value in raw.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    ignoredTargetColumns.Add(value);
                }

                if (ignoredTargetColumns.Count == 0)
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "value for --ignore-target-columns must include at least one column name.");
                }

                continue;
            }

            if (string.Equals(arg, "--data-type-conversion-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "missing value for --data-type-conversion-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(dataTypeConversionWorkspacePath))
                {
                    return (
                        false,
                        transformWorkspacePath,
                        sourceSchemaWorkspacePaths.ToArray(),
                        targetSchemaWorkspacePath,
                        executeSystemName,
                        executeSystemDefaultSchemaName,
                        newWorkspacePath,
                        ignoredTargetColumns.ToArray(),
                        dataTypeConversionWorkspacePath,
                        "--data-type-conversion-workspace can only be provided once.");
                }

                dataTypeConversionWorkspacePath = args[++i];
                continue;
            }

            return (
                false,
                transformWorkspacePath,
                sourceSchemaWorkspacePaths.ToArray(),
                targetSchemaWorkspacePath,
                executeSystemName,
                executeSystemDefaultSchemaName,
                newWorkspacePath,
                ignoredTargetColumns.ToArray(),
                dataTypeConversionWorkspacePath,
                $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(transformWorkspacePath))
        {
            return (
                false,
                transformWorkspacePath,
                sourceSchemaWorkspacePaths.ToArray(),
                targetSchemaWorkspacePath,
                executeSystemName,
                executeSystemDefaultSchemaName,
                newWorkspacePath,
                ignoredTargetColumns.ToArray(),
                dataTypeConversionWorkspacePath,
                "missing required option --transform-workspace <path>.");
        }

        if (sourceSchemaWorkspacePaths.Count == 0)
        {
            return (
                false,
                transformWorkspacePath,
                sourceSchemaWorkspacePaths.ToArray(),
                targetSchemaWorkspacePath,
                executeSystemName,
                executeSystemDefaultSchemaName,
                newWorkspacePath,
                ignoredTargetColumns.ToArray(),
                dataTypeConversionWorkspacePath,
                "missing required option --source-schema <path>.");
        }

        if (string.IsNullOrWhiteSpace(targetSchemaWorkspacePath))
        {
            return (
                false,
                transformWorkspacePath,
                sourceSchemaWorkspacePaths.ToArray(),
                targetSchemaWorkspacePath,
                executeSystemName,
                executeSystemDefaultSchemaName,
                newWorkspacePath,
                ignoredTargetColumns.ToArray(),
                dataTypeConversionWorkspacePath,
                "missing required option --target-schema <path>.");
        }

        if (string.IsNullOrWhiteSpace(executeSystemName))
        {
            return (
                false,
                transformWorkspacePath,
                sourceSchemaWorkspacePaths.ToArray(),
                targetSchemaWorkspacePath,
                executeSystemName,
                executeSystemDefaultSchemaName,
                newWorkspacePath,
                ignoredTargetColumns.ToArray(),
                dataTypeConversionWorkspacePath,
                "missing required option --execute-system <name>.");
        }

        if (string.IsNullOrWhiteSpace(newWorkspacePath))
        {
            return (
                false,
                transformWorkspacePath,
                sourceSchemaWorkspacePaths.ToArray(),
                targetSchemaWorkspacePath,
                executeSystemName,
                executeSystemDefaultSchemaName,
                newWorkspacePath,
                ignoredTargetColumns.ToArray(),
                dataTypeConversionWorkspacePath,
                "missing required option --new-workspace <path>.");
        }

        return (
            true,
            transformWorkspacePath,
            sourceSchemaWorkspacePaths.ToArray(),
            targetSchemaWorkspacePath,
            executeSystemName,
            executeSystemDefaultSchemaName,
            newWorkspacePath,
            ignoredTargetColumns.ToArray(),
            dataTypeConversionWorkspacePath,
            string.Empty);
    }

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        CliHelpRenderer.WriteAppHelp(Presenter, Cli);
    }

    private static void PrintBindHelp()
    {
        PrintCommandHelp("bind");
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
            renderedDetails.AddRange(details.Where(static detail => !string.IsNullOrWhiteSpace(detail)));
        }

        renderedDetails.Add($"Next: {next}");
        Presenter.WriteFailure(message, renderedDetails);
        return exitCode;
    }
}
