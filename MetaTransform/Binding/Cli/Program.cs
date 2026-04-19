using Meta.Core.Presentation;
using MetaBi.Cli.Common;
using MetaTransform.Binding;

internal static class Program
{
    private static readonly ConsolePresenter Presenter = new();

    static Task<int> Main(string[] args)
    {
        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return Task.FromResult(0);
        }

        if (string.Equals(args[0], "bind", StringComparison.OrdinalIgnoreCase))
        {
            return RunBindAsync(args, startIndex: 1);
        }

        return Task.FromResult(Fail($"unknown command '{args[0]}'.", "meta-transform-binding help"));
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
            return Task.FromResult(Fail(parse.ErrorMessage, "meta-transform-binding bind --help"));
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

            var result = new TransformBindingWorkspaceService().BindValidatedToWorkspace(
                parse.TransformWorkspacePath,
                parse.SourceSchemaWorkspacePaths,
                parse.TargetSchemaWorkspacePath,
                parse.ExecuteSystemName,
                parse.ExecuteSystemDefaultSchemaName,
                targetValidation.FullPath,
                validationOptions: options);

            Presenter.WriteOk($"Created {Path.GetFileName(result.WorkspacePath)}");
            Presenter.WriteKeyValueBlock("Binding", new[]
            {
                ("Scripts", result.TransformScriptCount.ToString()),
                ("Bindings", result.TransformBindingCount.ToString()),
                ("Sources", result.SourceCount.ToString()),
                ("Targets", result.TargetCount.ToString()),
                ("SourceRowsets", result.SourceRowsetValidationCount.ToString()),
                ("TargetRowsets", result.TargetRowsetValidationCount.ToString()),
                ("SourceColumns", result.SourceColumnValidationCount.ToString()),
                ("TargetColumns", result.TargetColumnValidationCount.ToString()),
                ("IgnoredTargetColumns", parse.IgnoredTargetColumns.Length.ToString()),
                ("Workspace", result.WorkspacePath)
            });

            return Task.FromResult(0);
        }
        catch (TransformBindingValidationException ex)
        {
            return Task.FromResult(Fail(
                "binding validation failed.",
                "fix the schema or transform contract mismatch and retry.",
                5,
                new[]
                {
                    $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                    $"  SourceSchemas: {string.Join(", ", parse.SourceSchemaWorkspacePaths.Select(Path.GetFullPath))}",
                    $"  TargetSchema: {Path.GetFullPath(parse.TargetSchemaWorkspacePath)}",
                    $"  ExecuteSystem: {parse.ExecuteSystemName}",
                    $"  ExecuteSystemDefaultSchemaName: {(string.IsNullOrWhiteSpace(parse.ExecuteSystemDefaultSchemaName) ? "<none>" : parse.ExecuteSystemDefaultSchemaName)}",
                    $"  Code: {ex.Code}",
                    $"  {ex.Message}"
                }));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Fail(
                "binding workspace generation failed.",
                "check transform/source-schema/target-schema inputs and retry.",
                4,
                new[]
                {
                    $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                    $"  SourceSchemas: {string.Join(", ", parse.SourceSchemaWorkspacePaths.Select(Path.GetFullPath))}",
                    $"  TargetSchema: {Path.GetFullPath(parse.TargetSchemaWorkspacePath)}",
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
                        "value for --ignore-target-columns must include at least one column name.");
                }

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
        Presenter.WriteUsage("meta-transform-binding <command> [options]");
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog(
            "Commands:",
            new[]
            {
                ("bind", "Bind all transform scripts and validate against source/target schema contracts into a new workspace."),
                ("help", "Show this help.")
            });
        Presenter.WriteNext("meta-transform-binding bind --help");
    }

    private static void PrintBindHelp()
    {
        Presenter.WriteInfo("Command: bind");
        Presenter.WriteUsage("meta-transform-binding bind --transform-workspace <path> --source-schema <path> [--source-schema <path> ...] --target-schema <path> --execute-system <name> --new-workspace <path> [--execute-system-default-schema-name <schema>] [--ignore-target-columns <col[,col...]>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  bind is atomic: it binds and validates in one run.");
        Presenter.WriteInfo("  If binding or validation fails, no binding workspace is created.");
        Presenter.WriteInfo("  bind processes all transform scripts in the transform workspace.");
        Presenter.WriteInfo("  Target SQL identifier is read from TransformScript.TargetSqlIdentifier.");
        Presenter.WriteInfo("  Source schema workspaces are repeatable; target schema workspace is single.");
        Presenter.WriteInfo("  Every schema workspace must contain exactly one system.");
        Presenter.WriteInfo("  --execute-system defines execution context for one/two-part source identifiers.");
        Presenter.WriteInfo("  --execute-system-default-schema-name is required when any one-part source identifier exists.");
        Presenter.WriteInfo("  --ignore-target-columns excludes named non-identity target columns from target conformance checks.");
        Presenter.WriteInfo("  Ignored names must exist on each target table or bind fails explicitly.");
        Presenter.WriteInfo("Example:");
        Presenter.WriteInfo("  meta-transform-binding bind --transform-workspace .\\TransformWS --source-schema .\\SourceSchemaWS --target-schema .\\TargetSchemaWS --execute-system SalesDb --new-workspace .\\BindingWS");
        Presenter.WriteInfo("  meta-transform-binding bind --transform-workspace .\\TransformWS --source-schema .\\SalesSchemaWS --source-schema .\\ReferenceSchemaWS --target-schema .\\WarehouseSchemaWS --execute-system WarehouseDb --execute-system-default-schema-name dbo --new-workspace .\\BindingWS --ignore-target-columns LoadUtc,RunId");
    }

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
