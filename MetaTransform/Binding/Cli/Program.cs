using Meta.Core.Presentation;
using MetaBi.Cli.Common;
using MetaTransform.Binding;

internal static class Program
{
    private const string DefaultLanguageProfileId = "MetaTransformSqlServer_v1";
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
            var result = new TransformBindingWorkspaceService().BindValidatedToWorkspace(
                parse.TransformWorkspacePath,
                parse.SchemaWorkspacePath,
                targetValidation.FullPath,
                parse.Name,
                parse.LanguageProfileId ?? DefaultLanguageProfileId,
                TransformBindingValidationOptions.Create(parse.IgnoredTargetColumns));

            Presenter.WriteOk($"Created {Path.GetFileName(result.WorkspacePath)}");
            Presenter.WriteKeyValueBlock("Binding", new[]
            {
                ("Transform", result.TransformScriptName),
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
                    $"  SchemaWorkspace: {Path.GetFullPath(parse.SchemaWorkspacePath)}",
                    $"  Code: {ex.Code}",
                    $"  {ex.Message}"
                }));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Fail(
                "binding workspace generation failed.",
                "check the transform workspace, schema workspace, selected script, and target workspace, then retry.",
                4,
                new[]
                {
                    $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                    $"  SchemaWorkspace: {Path.GetFullPath(parse.SchemaWorkspacePath)}",
                    $"  BindingWorkspace: {targetValidation.FullPath}",
                    $"  {ex.Message}"
                }));
        }
    }

    private static (
        bool Ok,
        string TransformWorkspacePath,
        string SchemaWorkspacePath,
        string NewWorkspacePath,
        string? Name,
        string? LanguageProfileId,
        string[] IgnoredTargetColumns,
        string ErrorMessage) ParseBindArgs(
        string[] args,
        int startIndex)
    {
        var transformWorkspacePath = string.Empty;
        var schemaWorkspacePath = string.Empty;
        var newWorkspacePath = string.Empty;
        string? name = null;
        string? languageProfileId = null;
        var ignoredTargetColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--transform-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "missing value for --transform-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(transformWorkspacePath))
                {
                    return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "--transform-workspace can only be provided once.");
                }

                transformWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--schema-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "missing value for --schema-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(schemaWorkspacePath))
                {
                    return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "--schema-workspace can only be provided once.");
                }

                schemaWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "missing value for --new-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(newWorkspacePath))
                {
                    return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "--new-workspace can only be provided once.");
                }

                newWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "missing value for --name.");
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "--name can only be provided once.");
                }

                name = args[++i];
                continue;
            }

            if (string.Equals(arg, "--language-profile", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "missing value for --language-profile.");
                }

                if (!string.IsNullOrWhiteSpace(languageProfileId))
                {
                    return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "--language-profile can only be provided once.");
                }

                languageProfileId = args[++i];
                continue;
            }

            if (string.Equals(arg, "--ignore-target-columns", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "missing value for --ignore-target-columns.");
                }

                var raw = args[++i];
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "value for --ignore-target-columns cannot be blank.");
                }

                foreach (var value in raw.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    ignoredTargetColumns.Add(value);
                }

                if (ignoredTargetColumns.Count == 0)
                {
                    return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "value for --ignore-target-columns must include at least one column name.");
                }

                continue;
            }

            return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(transformWorkspacePath))
        {
            return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "missing required option --transform-workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(schemaWorkspacePath))
        {
            return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "missing required option --schema-workspace <path>.");
        }

        if (string.IsNullOrWhiteSpace(newWorkspacePath))
        {
            return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), "missing required option --new-workspace <path>.");
        }

        return (true, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, name, languageProfileId, ignoredTargetColumns.ToArray(), string.Empty);
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
                ("bind", "Bind one transform script and validate it against schema into a new workspace."),
                ("help", "Show this help.")
            });
        Presenter.WriteNext("meta-transform-binding bind --help");
    }

    private static void PrintBindHelp()
    {
        Presenter.WriteInfo("Command: bind");
        Presenter.WriteUsage("meta-transform-binding bind --transform-workspace <path> --schema-workspace <path> --new-workspace <path> [--name <name>] [--language-profile <id>] [--ignore-target-columns <col[,col...]>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  bind is atomic: it binds and validates in one run.");
        Presenter.WriteInfo("  If binding or validation fails, no binding workspace is created.");
        Presenter.WriteInfo("  Target SQL identifier is read from TransformScript.TargetSqlIdentifier.");
        Presenter.WriteInfo("  Target must be table, schema.table, or database.schema.table.");
        Presenter.WriteInfo("  If the transform workspace contains multiple scripts, --name is required.");
        Presenter.WriteInfo($"  --language-profile defaults to {DefaultLanguageProfileId} for this CLI run.");
        Presenter.WriteInfo("  If provided, --language-profile overrides both that CLI default and TransformScript.LanguageProfileId.");
        Presenter.WriteInfo("  --ignore-target-columns excludes named non-identity target columns from target conformance checks.");
        Presenter.WriteInfo("  Ignored names must exist on each target table or bind fails explicitly.");
        Presenter.WriteInfo("Example:");
        Presenter.WriteInfo("  meta-transform-binding bind --transform-workspace .\\TransformWorkspace --schema-workspace .\\SchemaWorkspace --name sales.CustomerOrderSummary --new-workspace .\\BindingWorkspace");
        Presenter.WriteInfo("  meta-transform-binding bind --transform-workspace .\\TransformWorkspace --schema-workspace .\\SchemaWorkspace --name sales.CustomerOrderSummary --new-workspace .\\BindingWorkspace --ignore-target-columns LoadUtc,RunId");
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
