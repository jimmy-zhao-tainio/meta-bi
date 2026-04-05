using Meta.Core.Presentation;
using MetaBi.Cli.Common;
using MetaTransformScript.Sql;

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

        if (string.Equals(args[0], "from", StringComparison.OrdinalIgnoreCase))
        {
            return await RunFromAsync(args).ConfigureAwait(false);
        }

        if (string.Equals(args[0], "to", StringComparison.OrdinalIgnoreCase))
        {
            return await RunToAsync(args).ConfigureAwait(false);
        }

        return Fail($"unknown command '{args[0]}'.", "meta-transform-script help");
    }

    private static async Task<int> RunFromAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintFromHelp();
            return 0;
        }

        var service = new MetaTransformScriptSqlService();

        if (string.Equals(args[1], "sql-path", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 3 && IsHelpToken(args[2]))
            {
                PrintFromSqlPathHelp();
                return 0;
            }

            var parse = ParseFromSqlPathArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, "meta-transform-script from sql-path --help");
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

            try
            {
                var result = await service.ImportFromSqlPathToWorkspaceAsync(
                    parse.Path,
                    targetValidation.FullPath).ConfigureAwait(false);

                Presenter.WriteOk($"Created {Path.GetFileName(result.WorkspacePath)}");
                Presenter.WriteKeyValueBlock("Import", new[]
                {
                    ("Scripts", result.ScriptCount.ToString()),
                    ("Workspace", result.WorkspacePath)
                });
                return 0;
            }
            catch (MetaTransformScriptSqlImportException ex)
            {
                return Fail(
                    GetImportFailureMessage("sql-path", ex.Kind),
                    GetImportFailureNext("sql-path", ex.Kind),
                    4,
                    new[]
                    {
                        $"  Path: {Path.GetFullPath(parse.Path)}",
                        $"  Workspace: {targetValidation.FullPath}",
                        $"  {ex.Message}"
                    });
            }
            catch (Exception ex)
            {
                return Fail(
                    "sql-path import failed.",
                    "check the SQL path and target workspace, then retry.",
                    4,
                    new[]
                    {
                        $"  Path: {Path.GetFullPath(parse.Path)}",
                        $"  Workspace: {targetValidation.FullPath}",
                        $"  {ex.Message}"
                    });
            }
        }

        if (string.Equals(args[1], "sql-code", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 3 && IsHelpToken(args[2]))
            {
                PrintFromSqlCodeHelp();
                return 0;
            }

            var parse = ParseFromSqlCodeArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, "meta-transform-script from sql-code --help");
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

            try
            {
                var result = await service.ImportFromSqlCodeToWorkspaceAsync(
                    parse.Code,
                    targetValidation.FullPath,
                    parse.Name).ConfigureAwait(false);

                Presenter.WriteOk($"Created {Path.GetFileName(result.WorkspacePath)}");
                Presenter.WriteKeyValueBlock("Import", new[]
                {
                    ("Scripts", result.ScriptCount.ToString()),
                    ("Workspace", result.WorkspacePath)
                });
                return 0;
            }
            catch (MetaTransformScriptSqlImportException ex)
            {
                return Fail(
                    GetImportFailureMessage("sql-code", ex.Kind),
                    GetImportFailureNext("sql-code", ex.Kind),
                    4,
                    new[]
                    {
                        $"  Workspace: {targetValidation.FullPath}",
                        $"  {ex.Message}"
                    });
            }
            catch (Exception ex)
            {
                return Fail(
                    "sql-code import failed.",
                    "check the SQL code and target workspace, then retry.",
                    4,
                    new[]
                    {
                        $"  Workspace: {targetValidation.FullPath}",
                        $"  {ex.Message}"
                    });
            }
        }

        return Fail($"unknown source '{args[1]}'.", "meta-transform-script from --help");
    }

    private static async Task<int> RunToAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintToHelp();
            return 0;
        }

        var service = new MetaTransformScriptSqlService();

        if (string.Equals(args[1], "sql-path", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 3 && IsHelpToken(args[2]))
            {
                PrintToSqlPathHelp();
                return 0;
            }

            var parse = ParseToSqlPathArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, "meta-transform-script to sql-path --help");
            }

            try
            {
                var result = await service.ExportToSqlPathAsync(
                    parse.WorkspacePath,
                    parse.OutputPath).ConfigureAwait(false);

                Presenter.WriteOk($"Exported {result.ScriptCount} transform script{(result.ScriptCount == 1 ? string.Empty : "s")}");
                Presenter.WriteKeyValueBlock("Output", new[]
                {
                    ("Scripts", result.ScriptCount.ToString()),
                    ("Path", result.OutputPath)
                });
                return 0;
            }
            catch (Exception ex)
            {
                return Fail(
                    "sql-path export failed.",
                    "check the workspace, output path, and selected script, then retry.",
                    4,
                    new[]
                    {
                        $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}",
                        $"  Output: {Path.GetFullPath(parse.OutputPath)}",
                        $"  {ex.Message}"
                    });
            }
        }

        if (string.Equals(args[1], "sql-code", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 3 && IsHelpToken(args[2]))
            {
                PrintToSqlCodeHelp();
                return 0;
            }

            var parse = ParseToSqlCodeArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, "meta-transform-script to sql-code --help");
            }

            try
            {
                var sql = service.ExportToSqlCode(parse.WorkspacePath, parse.Name);
                Console.Out.Write(sql);
                if (!sql.EndsWith(Environment.NewLine, StringComparison.Ordinal))
                {
                    Console.Out.WriteLine();
                }

                return 0;
            }
            catch (Exception ex)
            {
                return Fail(
                    "sql-code export failed.",
                    "check the workspace and selected script, then retry.",
                    4,
                    new[]
                    {
                        $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}",
                        $"  {ex.Message}"
                    });
            }
        }

        return Fail($"unknown target '{args[1]}'.", "meta-transform-script to --help");
    }

    private static (bool Ok, string Path, string NewWorkspacePath, string ErrorMessage) ParseFromSqlPathArgs(string[] args, int startIndex)
    {
        var path = string.Empty;
        var newWorkspacePath = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--path", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, path, newWorkspacePath, "missing value for --path.");
                if (!string.IsNullOrWhiteSpace(path)) return (false, path, newWorkspacePath, "--path can only be provided once.");
                path = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, path, newWorkspacePath, "missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, path, newWorkspacePath, "--new-workspace can only be provided once.");
                newWorkspacePath = args[++i];
                continue;
            }

            return (false, path, newWorkspacePath, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(path)) return (false, path, newWorkspacePath, "missing required option --path <path>.");
        if (string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, path, newWorkspacePath, "missing required option --new-workspace <path>.");
        return (true, path, newWorkspacePath, string.Empty);
    }

    private static (bool Ok, string Code, string NewWorkspacePath, string? Name, string ErrorMessage) ParseFromSqlCodeArgs(string[] args, int startIndex)
    {
        var code = string.Empty;
        var newWorkspacePath = string.Empty;
        string? name = null;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--code", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, code, newWorkspacePath, name, "missing value for --code.");
                if (!string.IsNullOrWhiteSpace(code)) return (false, code, newWorkspacePath, name, "--code can only be provided once.");
                code = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, code, newWorkspacePath, name, "missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, code, newWorkspacePath, name, "--new-workspace can only be provided once.");
                newWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, code, newWorkspacePath, name, "missing value for --name.");
                if (!string.IsNullOrWhiteSpace(name)) return (false, code, newWorkspacePath, name, "--name can only be provided once.");
                name = args[++i];
                continue;
            }

            return (false, code, newWorkspacePath, name, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(code)) return (false, code, newWorkspacePath, name, "missing required option --code <sql>.");
        if (string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, code, newWorkspacePath, name, "missing required option --new-workspace <path>.");
        return (true, code, newWorkspacePath, name, string.Empty);
    }

    private static (bool Ok, string WorkspacePath, string OutputPath, string ErrorMessage) ParseToSqlPathArgs(string[] args, int startIndex)
    {
        var workspacePath = ".";
        var workspaceSpecified = false;
        var outputPath = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, outputPath, "missing value for --workspace.");
                if (workspaceSpecified) return (false, workspacePath, outputPath, "--workspace can only be provided once.");
                workspacePath = args[++i];
                workspaceSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--out", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, outputPath, "missing value for --out.");
                if (!string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, outputPath, "--out can only be provided once.");
                outputPath = args[++i];
                continue;
            }

            return (false, workspacePath, outputPath, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, outputPath, "missing required option --out <path>.");
        return (true, workspacePath, outputPath, string.Empty);
    }

    private static (bool Ok, string WorkspacePath, string? Name, string ErrorMessage) ParseToSqlCodeArgs(string[] args, int startIndex)
    {
        var workspacePath = ".";
        var workspaceSpecified = false;
        string? name = null;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, name, "missing value for --workspace.");
                if (workspaceSpecified) return (false, workspacePath, name, "--workspace can only be provided once.");
                workspacePath = args[++i];
                workspaceSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, name, "missing value for --name.");
                if (!string.IsNullOrWhiteSpace(name)) return (false, workspacePath, name, "--name can only be provided once.");
                name = args[++i];
                continue;
            }

            return (false, workspacePath, name, $"unknown option '{arg}'.");
        }

        return (true, workspacePath, name, string.Empty);
    }

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        Presenter.WriteUsage("meta-transform-script <command> [options]");
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog(
            "Commands:",
            new[]
            {
                ("from", "Import SQL files or SQL code into a new workspace."),
                ("to", "Emit SQL files or SQL code from a MetaTransformScript workspace."),
                ("help", "Show this help.")
            });
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteNext("meta-transform-script from --help");
    }

    private static void PrintFromHelp()
    {
        Presenter.WriteInfo("Command: from");
        Presenter.WriteUsage("meta-transform-script from <source> [options]");
        Presenter.WriteCommandCatalog(
            "Sources:",
            new[]
            {
                ("sql-path", "Import one .sql file or a folder of .sql files into a new workspace."),
                ("sql-code", "Import SQL text into a new workspace.")
            });
        Presenter.WriteInfo("Common option:");
        Presenter.WriteInfo("  --new-workspace <path>  Required for import commands.");
        Presenter.WriteInfo("Examples:");
        Presenter.WriteInfo("  meta-transform-script from sql-path --path .\\Views --new-workspace .\\TransformWorkspace");
        Presenter.WriteInfo("  meta-transform-script from sql-code --code \"select 1 as A\" --name dbo.v_inline --new-workspace .\\TransformWorkspace");
        Presenter.WriteNext("meta-transform-script from sql-path --help");
    }

    private static void PrintToHelp()
    {
        Presenter.WriteInfo("Command: to");
        Presenter.WriteUsage("meta-transform-script to <target> [options]");
        Presenter.WriteCommandCatalog(
            "Targets:",
            new[]
            {
                ("sql-path", "Emit CREATE VIEW scripts to a file or folder."),
                ("sql-code", "Emit one transform script body as SQL text.")
            });
        Presenter.WriteNext("meta-transform-script to sql-path --help");
    }

    private static void PrintFromSqlPathHelp()
    {
        Presenter.WriteInfo("Command: from sql-path");
        Presenter.WriteUsage("meta-transform-script from sql-path --path <path> --new-workspace <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Accepts either one .sql file or a folder of .sql files.");
        Presenter.WriteInfo("  CREATE VIEW wrappers are accepted as envelopes.");
        Presenter.WriteInfo("  SET statements and GO-separated batches are tolerated as long as supported CREATE VIEW statements can be found.");
        Presenter.WriteInfo("  Explicit view column lists are captured.");
        Presenter.WriteInfo("  View options and WITH CHECK OPTION are still rejected.");
        Presenter.WriteInfo("  Bare SELECT files are not accepted on sql-path import. Use CREATE VIEW wrappers in files.");
    }

    private static void PrintFromSqlCodeHelp()
    {
        Presenter.WriteInfo("Command: from sql-code");
        Presenter.WriteUsage("meta-transform-script from sql-code --code <sql> --new-workspace <path> [--name <name>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Imports SQL text directly into a new workspace.");
        Presenter.WriteInfo("  --name is required when the code is a bare SELECT body without a CREATE VIEW wrapper.");
    }

    private static void PrintToSqlPathHelp()
    {
        Presenter.WriteInfo("Command: to sql-path");
        Presenter.WriteUsage("meta-transform-script to sql-path [--workspace <path>] --out <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Emits CREATE VIEW wrappers plus GO separators.");
        Presenter.WriteInfo("  If --out ends with .sql, all scripts are emitted into one file.");
        Presenter.WriteInfo("  Otherwise --out is treated as a target folder and must be empty or missing.");
    }

    private static void PrintToSqlCodeHelp()
    {
        Presenter.WriteInfo("Command: to sql-code");
        Presenter.WriteUsage("meta-transform-script to sql-code [--workspace <path>] [--name <name>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Emits the SELECT body only, not the CREATE VIEW wrapper.");
        Presenter.WriteInfo("  If the workspace contains multiple scripts, --name is required.");
    }

    private static string GetImportFailureMessage(
        string sourceLabel,
        MetaTransformScriptSqlImportFailureKind kind) =>
        kind switch
        {
            MetaTransformScriptSqlImportFailureKind.SourcePathNotFound => $"{sourceLabel} import source was not found.",
            MetaTransformScriptSqlImportFailureKind.SourcePathHasNoSqlFiles => $"{sourceLabel} import source does not contain any .sql files.",
            MetaTransformScriptSqlImportFailureKind.ParseFailed => $"{sourceLabel} parse failed.",
            MetaTransformScriptSqlImportFailureKind.UnsupportedSql => $"{sourceLabel} import hit unsupported SQL.",
            MetaTransformScriptSqlImportFailureKind.InvalidSqlInput => $"{sourceLabel} import found unsupported SQL input shape.",
            _ => $"{sourceLabel} import failed."
        };

    private static string GetImportFailureNext(
        string sourceLabel,
        MetaTransformScriptSqlImportFailureKind kind) =>
        kind switch
        {
            MetaTransformScriptSqlImportFailureKind.SourcePathNotFound => sourceLabel == "sql-path"
                ? "check the SQL path and retry."
                : "check the SQL input and retry.",
            MetaTransformScriptSqlImportFailureKind.SourcePathHasNoSqlFiles => "point --path at a .sql file or a folder that contains .sql files, then retry.",
            MetaTransformScriptSqlImportFailureKind.ParseFailed => "fix the SQL syntax and retry.",
            MetaTransformScriptSqlImportFailureKind.UnsupportedSql => "remove unsupported wrapper options or unsupported SQL surface, then retry.",
            MetaTransformScriptSqlImportFailureKind.InvalidSqlInput => "provide CREATE VIEW wrappers, or use sql-code with --name for bare SELECT input, then retry.",
            _ => $"check the {sourceLabel} input and retry."
        };

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
