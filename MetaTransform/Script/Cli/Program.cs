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

        if (string.Equals(args[1], "sql-file", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 3 && IsHelpToken(args[2]))
            {
                PrintFromSqlFileHelp();
                return 0;
            }

            var parse = ParseFromSqlFileArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, "meta-transform-script from sql-file --help");
            }

            var fullPath = Path.GetFullPath(parse.Path);
            if (!File.Exists(fullPath))
            {
                return Fail(
                    "sql-file import source was not found.",
                    "check --path and retry.",
                    4,
                    new[] { $"  Path: {fullPath}" });
            }

            if (!string.Equals(Path.GetExtension(fullPath), ".sql", StringComparison.OrdinalIgnoreCase))
            {
                return Fail(
                    "sql-file import source must be a .sql file.",
                    "point --path at a .sql file and retry.",
                    4,
                    new[] { $"  Path: {fullPath}" });
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(parse.NewWorkspacePath))
                {
                    var targetValidation = CliNewWorkspaceTargetValidator.Validate(parse.NewWorkspacePath!);
                    if (!targetValidation.Ok)
                    {
                        return Fail(
                            targetValidation.ErrorMessage,
                            "choose a new folder or empty the target directory and retry.",
                            4,
                            targetValidation.Details);
                    }

                    var result = await service.ImportSingleSqlFileToWorkspaceAsync(
                        fullPath,
                        parse.TargetSqlIdentifier,
                        targetValidation.FullPath).ConfigureAwait(false);

                    Presenter.WriteOk($"Created {Path.GetFileName(result.WorkspacePath)}");
                    Presenter.WriteKeyValueBlock("Import", new[]
                    {
                        ("Scripts", result.ScriptCount.ToString()),
                        ("Workspace", result.WorkspacePath)
                    });
                    return 0;
                }

                var workspaceFullPath = Path.GetFullPath(parse.WorkspacePath!);
                var resultFromExisting = await service.AddSqlFileToWorkspaceAsync(
                    fullPath,
                    parse.TargetSqlIdentifier,
                    workspaceFullPath).ConfigureAwait(false);

                Presenter.WriteOk($"Updated {Path.GetFileName(resultFromExisting.WorkspacePath)}");
                Presenter.WriteKeyValueBlock("Import", new[]
                {
                    ("Added", "1"),
                    ("Scripts", resultFromExisting.ScriptCount.ToString()),
                    ("Workspace", resultFromExisting.WorkspacePath)
                });
                return 0;
            }
            catch (MetaTransformScriptSqlImportException ex)
            {
                return Fail(
                    GetImportFailureMessage("sql-file", ex.Kind),
                    GetImportFailureNext("sql-file", ex.Kind),
                    4,
                    new[]
                    {
                        $"  Path: {fullPath}",
                        $"  Target: {parse.TargetSqlIdentifier}",
                        $"  Workspace: {Path.GetFullPath(parse.NewWorkspacePath ?? parse.WorkspacePath ?? string.Empty)}",
                        $"  {ex.Message}"
                    });
            }
            catch (Exception ex)
            {
                return Fail(
                    "sql-file import failed.",
                    "check --path, --target, and workspace options, then retry.",
                    4,
                    new[]
                    {
                        $"  Path: {fullPath}",
                        $"  Target: {parse.TargetSqlIdentifier}",
                        $"  Workspace: {Path.GetFullPath(parse.NewWorkspacePath ?? parse.WorkspacePath ?? string.Empty)}",
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

            try
            {
                if (!string.IsNullOrWhiteSpace(parse.NewWorkspacePath))
                {
                    var targetValidation = CliNewWorkspaceTargetValidator.Validate(parse.NewWorkspacePath!);
                    if (!targetValidation.Ok)
                    {
                        return Fail(
                            targetValidation.ErrorMessage,
                            "choose a new folder or empty the target directory and retry.",
                            4,
                            targetValidation.Details);
                    }

                    var result = await service.ImportFromSqlCodeToWorkspaceAsync(
                        parse.Code,
                        parse.TargetSqlIdentifier,
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

                var workspaceFullPath = Path.GetFullPath(parse.WorkspacePath!);
                var resultFromExisting = await service.AddSqlCodeToWorkspaceAsync(
                    parse.Code,
                    parse.TargetSqlIdentifier,
                    workspaceFullPath,
                    parse.Name).ConfigureAwait(false);

                Presenter.WriteOk($"Updated {Path.GetFileName(resultFromExisting.WorkspacePath)}");
                Presenter.WriteKeyValueBlock("Import", new[]
                {
                    ("Added", "1"),
                    ("Scripts", resultFromExisting.ScriptCount.ToString()),
                    ("Workspace", resultFromExisting.WorkspacePath)
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
                        $"  Target: {parse.TargetSqlIdentifier}",
                        $"  Workspace: {Path.GetFullPath(parse.NewWorkspacePath ?? parse.WorkspacePath ?? string.Empty)}",
                        $"  {ex.Message}"
                    });
            }
            catch (Exception ex)
            {
                return Fail(
                    "sql-code import failed.",
                    "check --code, --target, and workspace options, then retry.",
                    4,
                    new[]
                    {
                        $"  Target: {parse.TargetSqlIdentifier}",
                        $"  Workspace: {Path.GetFullPath(parse.NewWorkspacePath ?? parse.WorkspacePath ?? string.Empty)}",
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

    private static (
        bool Ok,
        string Code,
        string TargetSqlIdentifier,
        string? NewWorkspacePath,
        string? WorkspacePath,
        string? Name,
        string ErrorMessage) ParseFromSqlCodeArgs(string[] args, int startIndex)
    {
        var code = string.Empty;
        var targetSqlIdentifier = string.Empty;
        var newWorkspacePath = string.Empty;
        var workspacePath = string.Empty;
        string? name = null;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--code", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, code, targetSqlIdentifier, null, null, name, "missing value for --code.");
                if (!string.IsNullOrWhiteSpace(code)) return (false, code, targetSqlIdentifier, null, null, name, "--code can only be provided once.");
                code = args[++i];
                continue;
            }

            if (string.Equals(arg, "--target", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, code, targetSqlIdentifier, null, null, name, "missing value for --target.");
                if (!string.IsNullOrWhiteSpace(targetSqlIdentifier)) return (false, code, targetSqlIdentifier, null, null, name, "--target can only be provided once.");
                targetSqlIdentifier = args[++i];
                continue;
            }

            if (string.Equals(arg, "--name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, code, targetSqlIdentifier, null, null, name, "missing value for --name.");
                if (!string.IsNullOrWhiteSpace(name)) return (false, code, targetSqlIdentifier, null, null, name, "--name can only be provided once.");
                name = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, code, targetSqlIdentifier, null, null, name, "missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, code, targetSqlIdentifier, null, null, name, "--new-workspace can only be provided once.");
                newWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, code, targetSqlIdentifier, null, null, name, "missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(workspacePath)) return (false, code, targetSqlIdentifier, null, null, name, "--workspace can only be provided once.");
                workspacePath = args[++i];
                continue;
            }

            return (false, code, targetSqlIdentifier, null, null, name, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(code)) return (false, code, targetSqlIdentifier, null, null, name, "missing required option --code <sql>.");
        if (string.IsNullOrWhiteSpace(targetSqlIdentifier)) return (false, code, targetSqlIdentifier, null, null, name, "missing required option --target <sql-identifier>.");
        if (string.IsNullOrWhiteSpace(newWorkspacePath) == string.IsNullOrWhiteSpace(workspacePath))
        {
            return (false, code, targetSqlIdentifier, null, null, name, "specify exactly one of --new-workspace <path> or --workspace <path>.");
        }

        return (
            true,
            code,
            targetSqlIdentifier,
            string.IsNullOrWhiteSpace(newWorkspacePath) ? null : newWorkspacePath,
            string.IsNullOrWhiteSpace(workspacePath) ? null : workspacePath,
            name,
            string.Empty);
    }

    private static (
        bool Ok,
        string Path,
        string TargetSqlIdentifier,
        string? NewWorkspacePath,
        string? WorkspacePath,
        string ErrorMessage) ParseFromSqlFileArgs(string[] args, int startIndex)
    {
        var path = string.Empty;
        var targetSqlIdentifier = string.Empty;
        var newWorkspacePath = string.Empty;
        var workspacePath = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--path", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, path, targetSqlIdentifier, null, null, "missing value for --path.");
                if (!string.IsNullOrWhiteSpace(path)) return (false, path, targetSqlIdentifier, null, null, "--path can only be provided once.");
                path = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, path, targetSqlIdentifier, null, null, "missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, path, targetSqlIdentifier, null, null, "--new-workspace can only be provided once.");
                newWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, path, targetSqlIdentifier, null, null, "missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(workspacePath)) return (false, path, targetSqlIdentifier, null, null, "--workspace can only be provided once.");
                workspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--target", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, path, targetSqlIdentifier, null, null, "missing value for --target.");
                if (!string.IsNullOrWhiteSpace(targetSqlIdentifier)) return (false, path, targetSqlIdentifier, null, null, "--target can only be provided once.");
                targetSqlIdentifier = args[++i];
                continue;
            }

            return (false, path, targetSqlIdentifier, null, null, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(path)) return (false, path, targetSqlIdentifier, null, null, "missing required option --path <path>.");
        if (string.IsNullOrWhiteSpace(targetSqlIdentifier)) return (false, path, targetSqlIdentifier, null, null, "missing required option --target <sql-identifier>.");
        if (string.IsNullOrWhiteSpace(newWorkspacePath) == string.IsNullOrWhiteSpace(workspacePath))
        {
            return (false, path, targetSqlIdentifier, null, null, "specify exactly one of --new-workspace <path> or --workspace <path>.");
        }

        return (
            true,
            path,
            targetSqlIdentifier,
            string.IsNullOrWhiteSpace(newWorkspacePath) ? null : newWorkspacePath,
            string.IsNullOrWhiteSpace(workspacePath) ? null : workspacePath,
            string.Empty);
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
                ("from", "Import SQL file/code into a new or existing workspace."),
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
                ("sql-file", "Import one .sql file with explicit target SQL identifier."),
                ("sql-code", "Import SQL text with explicit target SQL identifier.")
            });
        Presenter.WriteInfo("Common options:");
        Presenter.WriteInfo("  --target <sql-identifier>  Required. Supports table, schema.table, or database.schema.table.");
        Presenter.WriteInfo("  Specify exactly one of:");
        Presenter.WriteInfo("    --new-workspace <path>   Create a new workspace.");
        Presenter.WriteInfo("    --workspace <path>       Add one script to an existing workspace.");
        Presenter.WriteInfo("Examples:");
        Presenter.WriteInfo("  meta-transform-script from sql-file --path .\\SourceViews\\001_customer_order_summary\\view.sql --target sales.CustomerOrderSummary --new-workspace .\\TransformWorkspace");
        Presenter.WriteInfo("  meta-transform-script from sql-file --path .\\SourceViews\\002_invoice_window\\view.sql --target reporting.InvoiceWindow --workspace .\\TransformWorkspace");
        Presenter.WriteInfo("  meta-transform-script from sql-code --code \"select 1 as A\" --name dbo.v_inline --target dbo.TargetTable --new-workspace .\\TransformWorkspace");
        Presenter.WriteNext("meta-transform-script from sql-file --help");
        Presenter.WriteNext("meta-transform-script from sql-code --help");
    }

    private static void PrintToHelp()
    {
        Presenter.WriteInfo("Command: to");
        Presenter.WriteUsage("meta-transform-script to <target> [options]");
        Presenter.WriteCommandCatalog(
            "Targets:",
            new[]
            {
                ("sql-path", "Emit CREATE VIEW/CREATE FUNCTION scripts to a file or folder."),
                ("sql-code", "Emit one transform script body as SQL text.")
            });
        Presenter.WriteNext("meta-transform-script to sql-path --help");
    }

    private static void PrintFromSqlCodeHelp()
    {
        Presenter.WriteInfo("Command: from sql-code");
        Presenter.WriteUsage("meta-transform-script from sql-code --code <sql> --target <sql-identifier> (--new-workspace <path> | --workspace <path>) [--name <name>]");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Imports SQL text into a new workspace, or appends one script to an existing workspace.");
        Presenter.WriteInfo("  --name is required when the code is a bare SELECT body without a CREATE VIEW/CREATE FUNCTION wrapper.");
    }

    private static void PrintFromSqlFileHelp()
    {
        Presenter.WriteInfo("Command: from sql-file");
        Presenter.WriteUsage("meta-transform-script from sql-file --path <file.sql> --target <sql-identifier> (--new-workspace <path> | --workspace <path>)");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Imports one .sql file at a time.");
        Presenter.WriteInfo("  Folder-level import is intentionally not supported because every script must declare explicit --target.");
        Presenter.WriteInfo("  File input should use CREATE VIEW/CREATE FUNCTION wrappers for deterministic script naming.");
    }

    private static void PrintToSqlPathHelp()
    {
        Presenter.WriteInfo("Command: to sql-path");
        Presenter.WriteUsage("meta-transform-script to sql-path [--workspace <path>] --out <path>");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Emits CREATE VIEW/CREATE FUNCTION wrappers plus GO separators.");
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
            MetaTransformScriptSqlImportFailureKind.SourcePathNotFound => sourceLabel == "sql-file"
                ? "check the SQL path and retry."
                : "check the SQL input and retry.",
            MetaTransformScriptSqlImportFailureKind.ParseFailed => "fix the SQL syntax and retry.",
            MetaTransformScriptSqlImportFailureKind.UnsupportedSql => "remove unsupported wrapper options or unsupported SQL surface, then retry.",
            MetaTransformScriptSqlImportFailureKind.InvalidSqlInput => "provide CREATE VIEW/CREATE FUNCTION wrappers, or use sql-code with --name for bare SELECT input, then retry.",
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
