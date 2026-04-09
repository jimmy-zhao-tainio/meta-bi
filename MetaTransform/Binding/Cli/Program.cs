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

        return RunAsync(args);
    }

    private static Task<int> RunAsync(string[] args)
    {
        var parse = ParseArgs(args, startIndex: 0);
        if (!parse.Ok)
        {
            return Task.FromResult(Fail(parse.ErrorMessage, "meta-transform-binding --help"));
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
                parse.SchemaWorkspacePath,
                targetValidation.FullPath,
                parse.Targets,
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
                "check the transform workspace, schema workspace, selected script, and target workspace, then retry.",
                4,
                new[]
                {
                    $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                    $"  SchemaWorkspace: {Path.GetFullPath(parse.SchemaWorkspacePath)}",
                    $"  BindingWorkspace: {targetValidation.FullPath}",
                    $"  Targets: {string.Join(", ", parse.Targets)}",
                    $"  {ex.Message}"
                }));
        }
    }

    private static (bool Ok, string TransformWorkspacePath, string SchemaWorkspacePath, string NewWorkspacePath, IReadOnlyList<string> Targets, string? Name, string? LanguageProfileId, string ErrorMessage) ParseArgs(
        string[] args,
        int startIndex)
    {
        var transformWorkspacePath = string.Empty;
        var schemaWorkspacePath = string.Empty;
        var newWorkspacePath = string.Empty;
        var targets = new List<string>();
        string? name = null;
        string? languageProfileId = null;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--transform-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "missing value for --transform-workspace.");
                if (!string.IsNullOrWhiteSpace(transformWorkspacePath)) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "--transform-workspace can only be provided once.");
                transformWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--schema-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "missing value for --schema-workspace.");
                if (!string.IsNullOrWhiteSpace(schemaWorkspacePath)) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "--schema-workspace can only be provided once.");
                schemaWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "--new-workspace can only be provided once.");
                newWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--target", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "missing value for --target.");

                var target = args[++i];
                if (string.IsNullOrWhiteSpace(target)) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "--target value cannot be blank.");
                targets.Add(target);
                continue;
            }

            if (string.Equals(arg, "--name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "missing value for --name.");
                if (!string.IsNullOrWhiteSpace(name)) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "--name can only be provided once.");
                name = args[++i];
                continue;
            }

            if (string.Equals(arg, "--language-profile", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "missing value for --language-profile.");
                if (!string.IsNullOrWhiteSpace(languageProfileId)) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "--language-profile can only be provided once.");
                languageProfileId = args[++i];
                continue;
            }

            return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(transformWorkspacePath)) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "missing required option --transform-workspace <path>.");
        if (string.IsNullOrWhiteSpace(schemaWorkspacePath)) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "missing required option --schema-workspace <path>.");
        if (targets.Count == 0) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "missing required option --target <sql-identifier>.");
        if (string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, "missing required option --new-workspace <path>.");

        return (true, transformWorkspacePath, schemaWorkspacePath, newWorkspacePath, targets, name, languageProfileId, string.Empty);
    }

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        Presenter.WriteUsage("meta-transform-binding --transform-workspace <path> --schema-workspace <path> --target <sql-identifier> [--target <sql-identifier> ...] --new-workspace <path> [--name <name>] [--language-profile <id>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  The schema workspace is the single sanctioned schema input for the transform.");
        Presenter.WriteInfo("  It may contain both source and target objects.");
        Presenter.WriteInfo("  Provide one or more --target values as table, schema.table, or database.schema.table.");
        Presenter.WriteInfo("  If the transform workspace contains multiple scripts, --name is required.");
        Presenter.WriteInfo($"  --language-profile defaults to {DefaultLanguageProfileId} for this CLI run.");
        Presenter.WriteInfo("  If provided, --language-profile overrides both that CLI default and TransformScript.LanguageProfileId.");
        Presenter.WriteInfo("Example:");
        Presenter.WriteInfo("  meta-transform-binding --transform-workspace .\\TransformWorkspace --schema-workspace .\\SchemaWorkspace --target sales.CustomerOrderSummary --new-workspace .\\BindingWorkspace");
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
