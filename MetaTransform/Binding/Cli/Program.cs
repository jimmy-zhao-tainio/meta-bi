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

        if (string.Equals(args[0], "validate", StringComparison.OrdinalIgnoreCase))
        {
            return RunValidateAsync(args, startIndex: 1);
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
            var result = new TransformBindingWorkspaceService().BindToWorkspace(
                parse.TransformWorkspacePath,
                targetValidation.FullPath,
                parse.Name,
                parse.LanguageProfileId ?? DefaultLanguageProfileId);

            Presenter.WriteOk($"Created {Path.GetFileName(result.WorkspacePath)}");
            Presenter.WriteKeyValueBlock("Binding", new[]
            {
                ("Transform", result.TransformScriptName),
                ("Bindings", result.TransformBindingCount.ToString()),
                ("Sources", result.SourceCount.ToString()),
                ("Targets", result.TargetCount.ToString()),
                ("Issues", result.IssueCount.ToString()),
                ("Errors", result.ErrorCount.ToString()),
                ("Workspace", result.WorkspacePath)
            });

            if (result.IssueCount > 0)
            {
                Presenter.WriteWarning("binding completed with issues; inspect the binding workspace for details.");
            }

            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            return Task.FromResult(Fail(
                "binding workspace generation failed.",
                "check the transform workspace, selected script, and target workspace, then retry.",
                4,
                new[]
                {
                    $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                    $"  BindingWorkspace: {targetValidation.FullPath}",
                    $"  {ex.Message}"
                }));
        }
    }

    private static Task<int> RunValidateAsync(string[] args, int startIndex)
    {
        if (args.Length == startIndex || (args.Length > startIndex && IsHelpToken(args[startIndex])))
        {
            PrintValidateHelp();
            return Task.FromResult(0);
        }

        var parse = ParseValidateArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Task.FromResult(Fail(parse.ErrorMessage, "meta-transform-binding validate --help"));
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
            var result = new TransformBindingValidationWorkspaceService().ValidateWorkspace(
                parse.BindingWorkspacePath,
                parse.SchemaWorkspacePath,
                targetValidation.FullPath,
                TransformBindingValidationOptions.Create(parse.IgnoredTargetColumns));

            Presenter.WriteOk($"Created {Path.GetFileName(result.WorkspacePath)}");
            Presenter.WriteKeyValueBlock("Validation", new[]
            {
                ("Bindings", result.TransformBindingCount.ToString()),
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
                "validation failed.",
                "fix the schema or transform contract mismatch and retry.",
                5,
                new[]
                {
                    $"  BindingWorkspace: {Path.GetFullPath(parse.BindingWorkspacePath)}",
                    $"  SchemaWorkspace: {Path.GetFullPath(parse.SchemaWorkspacePath)}",
                    $"  Code: {ex.Code}",
                    $"  {ex.Message}"
                }));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Fail(
                "validation workspace generation failed.",
                "check the binding workspace, schema workspace, and target workspace, then retry.",
                4,
                new[]
                {
                    $"  BindingWorkspace: {Path.GetFullPath(parse.BindingWorkspacePath)}",
                    $"  SchemaWorkspace: {Path.GetFullPath(parse.SchemaWorkspacePath)}",
                    $"  ValidatedWorkspace: {targetValidation.FullPath}",
                    $"  {ex.Message}"
                }));
        }
    }

    private static (bool Ok, string TransformWorkspacePath, string NewWorkspacePath, string? Name, string? LanguageProfileId, string ErrorMessage) ParseBindArgs(
        string[] args,
        int startIndex)
    {
        var transformWorkspacePath = string.Empty;
        var newWorkspacePath = string.Empty;
        string? name = null;
        string? languageProfileId = null;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--transform-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, transformWorkspacePath, newWorkspacePath, name, languageProfileId, "missing value for --transform-workspace.");
                if (!string.IsNullOrWhiteSpace(transformWorkspacePath)) return (false, transformWorkspacePath, newWorkspacePath, name, languageProfileId, "--transform-workspace can only be provided once.");
                transformWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, transformWorkspacePath, newWorkspacePath, name, languageProfileId, "missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, transformWorkspacePath, newWorkspacePath, name, languageProfileId, "--new-workspace can only be provided once.");
                newWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, transformWorkspacePath, newWorkspacePath, name, languageProfileId, "missing value for --name.");
                if (!string.IsNullOrWhiteSpace(name)) return (false, transformWorkspacePath, newWorkspacePath, name, languageProfileId, "--name can only be provided once.");
                name = args[++i];
                continue;
            }

            if (string.Equals(arg, "--language-profile", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, transformWorkspacePath, newWorkspacePath, name, languageProfileId, "missing value for --language-profile.");
                if (!string.IsNullOrWhiteSpace(languageProfileId)) return (false, transformWorkspacePath, newWorkspacePath, name, languageProfileId, "--language-profile can only be provided once.");
                languageProfileId = args[++i];
                continue;
            }

            return (false, transformWorkspacePath, newWorkspacePath, name, languageProfileId, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(transformWorkspacePath)) return (false, transformWorkspacePath, newWorkspacePath, name, languageProfileId, "missing required option --transform-workspace <path>.");
        if (string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, transformWorkspacePath, newWorkspacePath, name, languageProfileId, "missing required option --new-workspace <path>.");

        return (true, transformWorkspacePath, newWorkspacePath, name, languageProfileId, string.Empty);
    }

    private static (bool Ok, string BindingWorkspacePath, string SchemaWorkspacePath, string NewWorkspacePath, string[] IgnoredTargetColumns, string ErrorMessage) ParseValidateArgs(
        string[] args,
        int startIndex)
    {
        var bindingWorkspacePath = string.Empty;
        var schemaWorkspacePath = string.Empty;
        var newWorkspacePath = string.Empty;
        var ignoredTargetColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--binding-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), "missing value for --binding-workspace.");
                if (!string.IsNullOrWhiteSpace(bindingWorkspacePath)) return (false, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), "--binding-workspace can only be provided once.");
                bindingWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--schema-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), "missing value for --schema-workspace.");
                if (!string.IsNullOrWhiteSpace(schemaWorkspacePath)) return (false, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), "--schema-workspace can only be provided once.");
                schemaWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), "missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), "--new-workspace can only be provided once.");
                newWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--ignore-target-columns", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), "missing value for --ignore-target-columns.");
                var raw = args[++i];
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return (false, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), "value for --ignore-target-columns cannot be blank.");
                }

                foreach (var name in raw.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    ignoredTargetColumns.Add(name);
                }

                if (ignoredTargetColumns.Count == 0)
                {
                    return (false, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), "value for --ignore-target-columns must include at least one column name.");
                }

                continue;
            }

            return (false, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(bindingWorkspacePath)) return (false, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), "missing required option --binding-workspace <path>.");
        if (string.IsNullOrWhiteSpace(schemaWorkspacePath)) return (false, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), "missing required option --schema-workspace <path>.");
        if (string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), "missing required option --new-workspace <path>.");

        return (true, bindingWorkspacePath, schemaWorkspacePath, newWorkspacePath, ignoredTargetColumns.ToArray(), string.Empty);
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
                ("bind", "Bind one transform script into a new binding workspace."),
                ("validate", "Validate a binding workspace against a schema workspace into a new workspace."),
                ("help", "Show this help.")
            });
        Presenter.WriteNext("meta-transform-binding bind --help");
        Presenter.WriteNext("meta-transform-binding validate --help");
    }

    private static void PrintBindHelp()
    {
        Presenter.WriteInfo("Command: bind");
        Presenter.WriteUsage("meta-transform-binding bind --transform-workspace <path> --new-workspace <path> [--name <name>] [--language-profile <id>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Target SQL identifier is read from TransformScript.TargetSqlIdentifier.");
        Presenter.WriteInfo("  Target must be table, schema.table, or database.schema.table.");
        Presenter.WriteInfo("  If the transform workspace contains multiple scripts, --name is required.");
        Presenter.WriteInfo($"  --language-profile defaults to {DefaultLanguageProfileId} for this CLI run.");
        Presenter.WriteInfo("  If provided, --language-profile overrides both that CLI default and TransformScript.LanguageProfileId.");
        Presenter.WriteInfo("Examples:");
        Presenter.WriteInfo("  meta-transform-binding bind --transform-workspace .\\TransformWorkspace --name sales.CustomerOrderSummary --new-workspace .\\BindingWorkspace");
    }

    private static void PrintValidateHelp()
    {
        Presenter.WriteInfo("Command: validate");
        Presenter.WriteUsage("meta-transform-binding validate --binding-workspace <path> --schema-workspace <path> --new-workspace <path> [--ignore-target-columns <col[,col...]>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Validate resolves source and target SQL identifiers against the schema workspace.");
        Presenter.WriteInfo("  Validate fails hard on any mismatch or ambiguous/not-found identifier.");
        Presenter.WriteInfo("  --ignore-target-columns excludes named non-identity target columns from target conformance checks.");
        Presenter.WriteInfo("  Ignored names must exist on each target table or validation fails explicitly.");
        Presenter.WriteInfo("  Output is a new binding workspace containing explicit validation link rows.");
        Presenter.WriteInfo("Example:");
        Presenter.WriteInfo("  meta-transform-binding validate --binding-workspace .\\BindingWorkspace --schema-workspace .\\SchemaWorkspace --new-workspace .\\ValidatedBindingWorkspace");
        Presenter.WriteInfo("  meta-transform-binding validate --binding-workspace .\\BindingWorkspace --schema-workspace .\\SchemaWorkspace --new-workspace .\\ValidatedBindingWorkspace --ignore-target-columns LoadUtc,RunId");
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
