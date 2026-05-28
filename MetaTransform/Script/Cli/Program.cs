using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaTransformScript.Sql;

internal static class Program
{
    private const string AppName = "meta-transform-script";

    private static readonly ConsolePresenter Presenter = new();
    private static readonly IReadOnlyList<CliCommandRoute> CommandRoutes = BuildCommandRoutes();
    private static readonly IReadOnlyDictionary<string, CliCommandRoute> CommandRoutesByName = CommandRoutes
        .ToDictionary(route => route.Definition.Name, StringComparer.OrdinalIgnoreCase);
    private static readonly CliAppDefinition Cli = new(
        AppName,
        new[]
        {
            "meta-transform-script <command> [options]"
        },
        CommandRoutes
            .Select(route => route.Definition)
            .Concat(new[]
            {
                CreateFromSqlFileCommand(),
                CreateFromSqlFilesCommand(),
                CreateFromSqlCodeCommand(),
                CreateToSqlPathCommand(),
                CreateToSqlCodeCommand()
            })
            .ToArray(),
        Next: "meta-transform-script from --help");

    private static IReadOnlyList<CliCommandRoute> BuildCommandRoutes() =>
        new[]
        {
            new CliCommandRoute(
                new CliCommandDefinition(
                    "help",
                    "Show this help.",
                    new[] { "meta-transform-script help" }),
                _ =>
                {
                    PrintHelp();
                    return Task.FromResult(0);
                }),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "from",
                    "Import SQL file/code into a new or existing workspace.",
                    new[] { "meta-transform-script from <source> [options]" },
                    Notes: new[]
                    {
                        "Sources: sql-file, sql-files, sql-code.",
                        "--target <sql-identifier> is required for view/select imports and not allowed for TVF or mutation imports.",
                        "Specify exactly one of --new-workspace <path> or --workspace <path>."
                    },
                    Examples: new[]
                    {
                        "meta-transform-script from sql-file --path .\\SourceViews\\001_customer_order_summary\\view.sql --target sales.CustomerOrderSummary --new-workspace .\\TransformWorkspace",
                        "meta-transform-script from sql-file --path .\\SourceViews\\002_invoice_window\\view.sql --target reporting.InvoiceWindow --workspace .\\TransformWorkspace",
                        "meta-transform-script from sql-files --manifest .\\import-manifest.tsv --new-workspace .\\TransformWorkspace --report .\\import-report.tsv --verbose",
                        "meta-transform-script from sql-code --code \"select 1 as A\" --name dbo.v_inline --target dbo.TargetTable --new-workspace .\\TransformWorkspace"
                    },
                    Next: "meta-transform-script from sql-file --help",
                    AdditionalNext: new[] { "meta-transform-script from sql-files --help", "meta-transform-script from sql-code --help" }),
                RunFromAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "to",
                    "Emit SQL files or SQL code from a MetaTransformScript workspace.",
                    new[] { "meta-transform-script to <target> [options]" },
                    Notes: new[]
                    {
                        "Targets: sql-path, sql-code."
                    },
                    Next: "meta-transform-script to sql-path --help",
                    AdditionalNext: new[] { "meta-transform-script to sql-code --help" }),
                RunToAsync)
        };

    private static CliCommandDefinition CreateFromSqlFileCommand() =>
        new(
            "from sql-file",
            "Import one .sql file.",
            new[] { "meta-transform-script from sql-file --path <file.sql> [--target <sql-identifier>] (--new-workspace <path> | --workspace <path>)" },
            new[]
            {
                new CliOptionDefinition("--path <file.sql>", "Required. SQL file to import."),
                new CliOptionDefinition("--target <sql-identifier>", "Required for CREATE VIEW imports; not allowed for inline CREATE FUNCTION or mutation statement imports."),
                new CliOptionDefinition("--new-workspace <path>", "Create a new MetaTransformScript workspace. Mutually exclusive with --workspace."),
                new CliOptionDefinition("--workspace <path>", "Add one script to an existing workspace. Mutually exclusive with --new-workspace.")
            },
            new[]
            {
                "Imports one .sql file at a time.",
                "Folder-level import is intentionally not supported.",
                "Bare mutation statement file names are used as transform script names."
            },
            ShowInCommandCatalog: false);

    private static CliCommandDefinition CreateFromSqlFilesCommand() =>
        new(
            "from sql-files",
            "Import many .sql files from an explicit manifest.",
            new[] { "meta-transform-script from sql-files --manifest <manifest.tsv> (--new-workspace <path> | --workspace <path>) [--report <report.tsv>] [--verbose]" },
            new[]
            {
                new CliOptionDefinition("--manifest <manifest.tsv>", "Required. TSV manifest with a Path column and optional Target column."),
                new CliOptionDefinition("--new-workspace <path>", "Create a new MetaTransformScript workspace. Mutually exclusive with --workspace."),
                new CliOptionDefinition("--workspace <path>", "Add successful imports to an existing workspace. Mutually exclusive with --new-workspace."),
                new CliOptionDefinition("--report <report.tsv>", "Write per-file Success/Failure rows with structured failure kind, summary, line, and column columns."),
                new CliOptionDefinition("--verbose", "Print one progress line per imported file.")
            },
            new[]
            {
                "Manifest paths are resolved relative to the manifest file.",
                "Each row is one import attempt. CREATE VIEW and bare SELECT rows must supply Target; inline TVF and scalar UDF rows must leave Target blank.",
                "The command continues after per-file failures, saves successful imports once, and exits nonzero when any file failed."
            },
            ShowInCommandCatalog: false);

    private static CliCommandDefinition CreateFromSqlCodeCommand() =>
        new(
            "from sql-code",
            "Import SQL text.",
            new[] { "meta-transform-script from sql-code --code <sql> [--target <sql-identifier>] (--new-workspace <path> | --workspace <path>) [--name <name>]" },
            new[]
            {
                new CliOptionDefinition("--code <sql>", "Required. SQL text to import."),
                new CliOptionDefinition("--target <sql-identifier>", "Required for bare SELECT and CREATE VIEW imports; not allowed for inline CREATE FUNCTION or mutation statement imports."),
                new CliOptionDefinition("--new-workspace <path>", "Create a new MetaTransformScript workspace. Mutually exclusive with --workspace."),
                new CliOptionDefinition("--workspace <path>", "Add one script to an existing workspace. Mutually exclusive with --new-workspace."),
                new CliOptionDefinition("--name <name>", "Required when the code is a bare SELECT or mutation statement without a CREATE wrapper.")
            },
            new[]
            {
                "Imports SQL text into a new workspace, or appends one script to an existing workspace."
            },
            ShowInCommandCatalog: false);

    private static CliCommandDefinition CreateToSqlPathCommand() =>
        new(
            "to sql-path",
            "Emit SQL scripts to a file or folder.",
            new[] { "meta-transform-script to sql-path [--workspace <path>] --out <path>" },
            new[]
            {
                new CliOptionDefinition("--workspace <path>", "MetaTransformScript workspace to export. Defaults to the current directory."),
                new CliOptionDefinition("--out <path>", "Required. Output .sql file or target folder.")
            },
            new[]
            {
                "Emits CREATE VIEW/CREATE FUNCTION wrappers where modeled; mutation statements emit as statements.",
                "If --out ends with .sql, all scripts are emitted into one file.",
                "Otherwise --out is treated as a target folder and must be empty or missing."
            },
            ShowInCommandCatalog: false);

    private static CliCommandDefinition CreateToSqlCodeCommand() =>
        new(
            "to sql-code",
            "Emit one transform script as SQL text.",
            new[] { "meta-transform-script to sql-code [--workspace <path>] [--name <name>]" },
            new[]
            {
                new CliOptionDefinition("--workspace <path>", "MetaTransformScript workspace to export. Defaults to the current directory."),
                new CliOptionDefinition("--name <name>", "Required when the workspace contains multiple scripts.")
            },
            new[]
            {
                "Emits one modeled statement without CREATE VIEW/inline TVF wrapping; scalar function scripts emit CREATE FUNCTION wrappers."
            },
            ShowInCommandCatalog: false);

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

        return Fail($"unknown command '{args[0]}'.", $"{AppName} help");
    }

    private static async Task<int> RunFromAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintCommandHelp("from");
            return 0;
        }

        var service = new MetaTransformScriptSqlService();

        if (string.Equals(args[1], "sql-file", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 3 && IsHelpToken(args[2]))
            {
                PrintCommandHelp("from sql-file");
                return 0;
            }

            var parse = ParseFromSqlFileArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, HelpCommand("from sql-file"));
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

                    using var activity = CliActivityLine.Start("Importing");
                    await service.ImportSingleSqlFileToWorkspaceAsync(
                        fullPath,
                        parse.TargetSqlIdentifier,
                        targetValidation.FullPath).ConfigureAwait(false);

                    activity.Succeed();
                    return 0;
                }

                var workspaceFullPath = Path.GetFullPath(parse.WorkspacePath!);
                using (var activity = CliActivityLine.Start("Importing"))
                {
                    await service.AddSqlFileToWorkspaceAsync(
                        fullPath,
                        parse.TargetSqlIdentifier,
                        workspaceFullPath).ConfigureAwait(false);

                    activity.Succeed();
                }

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
                        $"  Target: {DisplayTarget(parse.TargetSqlIdentifier)}",
                        $"  Workspace: {Path.GetFullPath(parse.NewWorkspacePath ?? parse.WorkspacePath ?? string.Empty)}",
                        $"  {ex.Message}"
                    });
            }
            catch (Exception ex)
            {
                return Fail(
                    "Cannot import SQL file.",
                    "check --path, --target rules, and workspace options, then retry.",
                    4,
                    new[]
                    {
                        $"  Path: {fullPath}",
                        $"  Target: {DisplayTarget(parse.TargetSqlIdentifier)}",
                        $"  Workspace: {Path.GetFullPath(parse.NewWorkspacePath ?? parse.WorkspacePath ?? string.Empty)}",
                        $"  {ex.Message}"
                    });
            }
        }

        if (string.Equals(args[1], "sql-files", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 3 && IsHelpToken(args[2]))
            {
                PrintCommandHelp("from sql-files");
                return 0;
            }

            var parse = ParseFromSqlFilesArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, HelpCommand("from sql-files"));
            }

            var manifestFullPath = Path.GetFullPath(parse.ManifestPath);
            if (!File.Exists(manifestFullPath))
            {
                return Fail(
                    "sql-files import manifest was not found.",
                    "check --manifest and retry.",
                    4,
                    new[] { $"  Manifest: {manifestFullPath}" });
            }

            IReadOnlyList<SqlFileImportRequest> requests;
            try
            {
                requests = ReadSqlFilesManifest(manifestFullPath);
            }
            catch (Exception ex)
            {
                return Fail(
                    "Cannot read sql-files import manifest.",
                    "fix the TSV manifest and retry.",
                    4,
                    new[] { $"  Manifest: {manifestFullPath}", $"  {ex.Message}" });
            }

            if (requests.Count == 0)
            {
                return Fail(
                    "sql-files import manifest did not contain any files.",
                    "add at least one data row with a Path value and retry.",
                    4,
                    new[] { $"  Manifest: {manifestFullPath}" });
            }

            try
            {
                var completedCount = 0;
                var failureCount = 0;
                var currentFileName = string.Empty;
                CliLiveLineRenderer? liveLine = null;

                void ReportProgress(SqlFileImportProgress progress)
                {
                    completedCount = progress.Index;
                    currentFileName = Path.GetFileName(progress.Path);
                    if (!progress.Success)
                    {
                        failureCount++;
                    }

                    if (!parse.Verbose)
                    {
                        return;
                    }

                    var status = progress.Success ? "ok" : "failed";
                    var suffix = progress.Success
                        ? string.Empty
                        : " - " + SummarizeImportFailure(progress.Message);
                    Console.Out.WriteLine(
                        $"{status} [{progress.Index}/{progress.Total}] {currentFileName}{suffix}");
                }

                ImportSqlFilesToWorkspaceResult result;
                try
                {
                    if (!parse.Verbose)
                    {
                        liveLine = CliLiveLineRenderer.TryStart(
                            () => BuildSqlFilesImportProgressLine(
                                completedCount,
                                requests.Count,
                                failureCount,
                                currentFileName),
                            delay: TimeSpan.Zero);
                    }

                    if (!string.IsNullOrWhiteSpace(parse.NewWorkspacePath))
                    {
                        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parse.NewWorkspacePath!);
                        if (!targetValidation.Ok)
                        {
                            liveLine?.Clear();
                            liveLine = null;
                            return Fail(
                                targetValidation.ErrorMessage,
                                "choose a new folder or empty the target directory and retry.",
                                4,
                                targetValidation.Details);
                        }

                        result = await service.ImportSqlFilesToNewWorkspaceAsync(
                                requests,
                                targetValidation.FullPath,
                                ReportProgress)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        result = await service.AddSqlFilesToWorkspaceAsync(
                                requests,
                                Path.GetFullPath(parse.WorkspacePath!),
                                ReportProgress)
                            .ConfigureAwait(false);
                    }

                    liveLine?.Complete(BuildSqlFilesImportProgressLine(
                        result.Successes.Count + result.Failures.Count,
                        requests.Count,
                        result.Failures.Count,
                        string.Empty));
                    liveLine = null;
                }
                finally
                {
                    liveLine?.Clear();
                }

                if (!string.IsNullOrWhiteSpace(parse.ReportPath))
                {
                    await WriteSqlFilesImportReportAsync(result, parse.ReportPath!).ConfigureAwait(false);
                }

                if (result.Failures.Count > 0)
                {
                    return Fail(
                        "SQL files import completed with failures.",
                        string.IsNullOrWhiteSpace(parse.ReportPath)
                            ? "inspect the failed files, fix the SQL or target mapping, and retry."
                            : "inspect the import report, fix the SQL or target mapping, and retry.",
                        4,
                        BuildSqlFilesFailureDetails(result, parse.ReportPath));
                }

                Presenter.WriteOk($"Imported {result.Successes.Count} SQL file{(result.Successes.Count == 1 ? string.Empty : "s")}");
                Presenter.WriteKeyValueBlock("Workspace", new[]
                {
                    ("Scripts", result.ScriptCount.ToString()),
                    ("Path", result.WorkspacePath)
                });
                if (!string.IsNullOrWhiteSpace(parse.ReportPath))
                {
                    Presenter.WriteKeyValueBlock("Report", new[] { ("Path", Path.GetFullPath(parse.ReportPath!)) });
                }

                return 0;
            }
            catch (Exception ex)
            {
                return Fail(
                    "Cannot import SQL files.",
                    "check --manifest, --workspace options, and report path, then retry.",
                    4,
                    new[]
                    {
                        $"  Manifest: {manifestFullPath}",
                        $"  Workspace: {Path.GetFullPath(parse.NewWorkspacePath ?? parse.WorkspacePath ?? string.Empty)}",
                        $"  {ex.Message}"
                    });
            }
        }

        if (string.Equals(args[1], "sql-code", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 3 && IsHelpToken(args[2]))
            {
                PrintCommandHelp("from sql-code");
                return 0;
            }

            var parse = ParseFromSqlCodeArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, HelpCommand("from sql-code"));
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

                    using var activity = CliActivityLine.Start("Importing");
                    await service.ImportFromSqlCodeToWorkspaceAsync(
                        parse.Code,
                        parse.TargetSqlIdentifier,
                        targetValidation.FullPath,
                        parse.Name).ConfigureAwait(false);

                    activity.Succeed();
                    return 0;
                }

                var workspaceFullPath = Path.GetFullPath(parse.WorkspacePath!);
                using (var activity = CliActivityLine.Start("Importing"))
                {
                    await service.AddSqlCodeToWorkspaceAsync(
                        parse.Code,
                        parse.TargetSqlIdentifier,
                        workspaceFullPath,
                        parse.Name).ConfigureAwait(false);

                    activity.Succeed();
                }

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
                        $"  Target: {DisplayTarget(parse.TargetSqlIdentifier)}",
                        $"  Workspace: {Path.GetFullPath(parse.NewWorkspacePath ?? parse.WorkspacePath ?? string.Empty)}",
                        $"  {ex.Message}"
                    });
            }
            catch (Exception ex)
            {
                return Fail(
                    "Cannot import SQL code.",
                    "check --code, --target rules, and workspace options, then retry.",
                    4,
                    new[]
                    {
                        $"  Target: {DisplayTarget(parse.TargetSqlIdentifier)}",
                        $"  Workspace: {Path.GetFullPath(parse.NewWorkspacePath ?? parse.WorkspacePath ?? string.Empty)}",
                        $"  {ex.Message}"
                    });
            }
        }

        return Fail($"unknown source '{args[1]}'.", HelpCommand("from"));
    }

    private static async Task<int> RunToAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintCommandHelp("to");
            return 0;
        }

        var service = new MetaTransformScriptSqlService();

        if (string.Equals(args[1], "sql-path", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length >= 3 && IsHelpToken(args[2]))
            {
                PrintCommandHelp("to sql-path");
                return 0;
            }

            var parse = ParseToSqlPathArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, HelpCommand("to sql-path"));
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
                    "Cannot export SQL files.",
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
                PrintCommandHelp("to sql-code");
                return 0;
            }

            var parse = ParseToSqlCodeArgs(args, 2);
            if (!parse.Ok)
            {
                return Fail(parse.ErrorMessage, HelpCommand("to sql-code"));
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
                    "Cannot export SQL code.",
                    "check the workspace and selected script, then retry.",
                    4,
                    new[]
                    {
                        $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}",
                        $"  {ex.Message}"
                    });
            }
        }

        return Fail($"unknown target '{args[1]}'.", HelpCommand("to"));
    }

    private sealed record FromSqlFilesArgs(
        bool Ok,
        string ManifestPath,
        string? NewWorkspacePath,
        string? WorkspacePath,
        string? ReportPath,
        bool Verbose,
        string ErrorMessage);

    private static FromSqlFilesArgs ParseFromSqlFilesArgs(string[] args, int startIndex)
    {
        var manifestPath = string.Empty;
        var newWorkspacePath = string.Empty;
        var workspacePath = string.Empty;
        string? reportPath = null;
        var verbose = false;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--manifest", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return new(false, manifestPath, null, null, reportPath, verbose, "missing value for --manifest.");
                if (!string.IsNullOrWhiteSpace(manifestPath)) return new(false, manifestPath, null, null, reportPath, verbose, "--manifest can only be provided once.");
                manifestPath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return new(false, manifestPath, null, null, reportPath, verbose, "missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(newWorkspacePath)) return new(false, manifestPath, null, null, reportPath, verbose, "--new-workspace can only be provided once.");
                newWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return new(false, manifestPath, null, null, reportPath, verbose, "missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(workspacePath)) return new(false, manifestPath, null, null, reportPath, verbose, "--workspace can only be provided once.");
                workspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--report", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return new(false, manifestPath, null, null, reportPath, verbose, "missing value for --report.");
                if (!string.IsNullOrWhiteSpace(reportPath)) return new(false, manifestPath, null, null, reportPath, verbose, "--report can only be provided once.");
                reportPath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--verbose", StringComparison.OrdinalIgnoreCase))
            {
                verbose = true;
                continue;
            }

            return new(false, manifestPath, null, null, reportPath, verbose, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(manifestPath)) return new(false, manifestPath, null, null, reportPath, verbose, "missing required option --manifest <manifest.tsv>.");
        if (string.IsNullOrWhiteSpace(newWorkspacePath) == string.IsNullOrWhiteSpace(workspacePath))
        {
            return new(false, manifestPath, null, null, reportPath, verbose, "specify exactly one of --new-workspace <path> or --workspace <path>.");
        }

        return new(
            true,
            manifestPath,
            string.IsNullOrWhiteSpace(newWorkspacePath) ? null : newWorkspacePath,
            string.IsNullOrWhiteSpace(workspacePath) ? null : workspacePath,
            reportPath,
            verbose,
            string.Empty);
    }

    private static IReadOnlyList<SqlFileImportRequest> ReadSqlFilesManifest(string manifestPath)
    {
        var manifestFullPath = Path.GetFullPath(manifestPath);
        var manifestDirectory = Path.GetDirectoryName(manifestFullPath) ?? Environment.CurrentDirectory;
        var rows = new List<SqlFileImportRequest>();
        var lines = File.ReadAllLines(manifestFullPath);
        string[]? headers = null;
        var pathColumnIndex = -1;
        var targetColumnIndex = -1;

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            if (headers is null)
            {
                headers = SplitTsvLine(line);
                pathColumnIndex = FindColumnIndex(headers, "Path");
                if (pathColumnIndex < 0)
                {
                    pathColumnIndex = FindColumnIndex(headers, "FilePath");
                }

                targetColumnIndex = FindColumnIndex(headers, "Target");
                if (pathColumnIndex < 0)
                {
                    throw new InvalidOperationException("Manifest header must contain a Path column.");
                }

                continue;
            }

            var cells = SplitTsvLine(line);
            var sourcePath = ReadManifestCell(cells, pathColumnIndex);
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                throw new InvalidOperationException($"Manifest line {lineIndex + 1} has a blank Path value.");
            }

            var resolvedPath = Path.IsPathFullyQualified(sourcePath)
                ? sourcePath
                : Path.GetFullPath(Path.Combine(manifestDirectory, sourcePath));
            var target = targetColumnIndex >= 0
                ? ReadManifestCell(cells, targetColumnIndex)
                : null;

            rows.Add(new SqlFileImportRequest(
                resolvedPath,
                string.IsNullOrWhiteSpace(target) ? null : target));
        }

        if (headers is null)
        {
            throw new InvalidOperationException("Manifest does not contain a header row.");
        }

        return rows;
    }

    private static int FindColumnIndex(string[] headers, string name)
    {
        for (var i = 0; i < headers.Length; i++)
        {
            if (string.Equals(headers[i].Trim(), name, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    private static string[] SplitTsvLine(string line) => line.Split('\t');

    private static string? ReadManifestCell(string[] cells, int index) =>
        index >= 0 && index < cells.Length ? cells[index].Trim() : null;

    private static async Task WriteSqlFilesImportReportAsync(
        ImportSqlFilesToWorkspaceResult result,
        string reportPath)
    {
        var fullPath = Path.GetFullPath(reportPath);
        var parentDirectory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(parentDirectory))
        {
            Directory.CreateDirectory(parentDirectory);
        }

        var lines = new List<string>
        {
            "Status\tIndex\tTotal\tPath\tTarget\tScriptName\tFailureKind\tErrorSummary\tLine\tColumn\tMessage"
        };

        foreach (var success in result.Successes)
        {
            lines.Add(string.Join(
                '\t',
                "Success",
                success.Index.ToString(),
                success.Total.ToString(),
                EscapeTsv(success.Path),
                EscapeTsv(success.TargetSqlIdentifier),
                EscapeTsv(success.ScriptName),
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty));
        }

        foreach (var failure in result.Failures)
        {
            lines.Add(string.Join(
                '\t',
                "Failure",
                failure.Index.ToString(),
                failure.Total.ToString(),
                EscapeTsv(failure.Path),
                EscapeTsv(failure.TargetSqlIdentifier),
                string.Empty,
                failure.Kind.ToString(),
                EscapeTsv(SummarizeImportFailure(failure.Message)),
                FormatNullableInt(failure.Line),
                FormatNullableInt(failure.Column),
                EscapeTsv(failure.Message)));
        }

        await File.WriteAllLinesAsync(fullPath, lines).ConfigureAwait(false);
    }

    private static IEnumerable<string> BuildSqlFilesFailureDetails(
        ImportSqlFilesToWorkspaceResult result,
        string? reportPath)
    {
        yield return $"  Workspace: {result.WorkspacePath}";
        yield return $"  Successes: {result.Successes.Count}";
        yield return $"  Failures: {result.Failures.Count}";
        if (!string.IsNullOrWhiteSpace(reportPath))
        {
            yield return $"  Report: {Path.GetFullPath(reportPath)}";
        }

        foreach (var failure in result.Failures.Take(10))
        {
            yield return $"  [{failure.Index}/{failure.Total}] {Path.GetFileName(failure.Path)}: {failure.Kind}: {SummarizeImportFailure(failure.Message)}";
        }

        if (result.Failures.Count > 10)
        {
            yield return $"  ...{result.Failures.Count - 10} more failure(s).";
        }
    }

    private static string BuildSqlFilesImportProgressLine(
        int completed,
        int total,
        int failures,
        string? currentFileName)
    {
        var meter = BuildMeter(completed, total);
        var file = string.IsNullOrWhiteSpace(currentFileName)
            ? string.Empty
            : " " + currentFileName;
        return $"Importing SQL files {meter} {completed} of {total}, {failures} failed{file}";
    }

    private static string BuildMeter(int completed, int total)
    {
        const int width = 20;
        if (total <= 0)
        {
            return "[" + new string('-', width) + "]";
        }

        var filled = Math.Clamp((int)Math.Round(width * (completed / (double)total)), 0, width);
        return "[" + new string('=', filled) + new string('-', width - filled) + "]";
    }

    private static string SummarizeImportFailure(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var lines = value
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .Select(static line => line.Trim())
            .Where(static line => line.Length > 0)
            .ToArray();

        foreach (var line in lines)
        {
            if (line.StartsWith("SQL import failed", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (line.Contains("Expected ", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("Unexpected ", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("unsupported", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("not supported", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("does not allow", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("line ", StringComparison.OrdinalIgnoreCase))
            {
                return line;
            }
        }

        return lines.FirstOrDefault() ?? string.Empty;
    }

    private static string EscapeTsv(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');

    private static string FormatNullableInt(int? value) =>
        value.HasValue ? value.Value.ToString() : string.Empty;

    private static (
        bool Ok,
        string Code,
        string? TargetSqlIdentifier,
        string? NewWorkspacePath,
        string? WorkspacePath,
        string? Name,
        string ErrorMessage) ParseFromSqlCodeArgs(string[] args, int startIndex)
    {
        var code = string.Empty;
        string? targetSqlIdentifier = null;
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
        string? TargetSqlIdentifier,
        string? NewWorkspacePath,
        string? WorkspacePath,
        string ErrorMessage) ParseFromSqlFileArgs(string[] args, int startIndex)
    {
        var path = string.Empty;
        string? targetSqlIdentifier = null;
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
        CliHelpRenderer.WriteAppHelp(Presenter, Cli);
    }

    private static void PrintCommandHelp(string commandName)
    {
        CliHelpRenderer.WriteCommandHelp(Presenter, Cli, Cli.GetCommand(commandName));
    }

    private static string HelpCommand(string commandName) => Cli.GetCommand(commandName).HelpCommand(Cli.Name);

    private static string GetImportFailureMessage(
        string sourceLabel,
        MetaTransformScriptSqlImportFailureKind kind) =>
        kind switch
        {
            MetaTransformScriptSqlImportFailureKind.SourcePathNotFound => $"{sourceLabel} import source was not found.",
            MetaTransformScriptSqlImportFailureKind.ParseFailed => $"Cannot parse {sourceLabel}.",
            MetaTransformScriptSqlImportFailureKind.UnsupportedSql => $"{sourceLabel} import hit unsupported SQL.",
            MetaTransformScriptSqlImportFailureKind.UnsupportedFunctionWrapper => $"{sourceLabel} import hit an unsupported CREATE FUNCTION wrapper.",
            MetaTransformScriptSqlImportFailureKind.LikelyTextEncodingMismatch => $"{sourceLabel} import found likely text encoding damage.",
            MetaTransformScriptSqlImportFailureKind.InvalidSqlInput => $"{sourceLabel} import found unsupported SQL input shape.",
            _ => $"Cannot import {sourceLabel}."
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
            MetaTransformScriptSqlImportFailureKind.UnsupportedFunctionWrapper => "skip scalar or multistatement function files, or import only inline table-valued functions.",
            MetaTransformScriptSqlImportFailureKind.LikelyTextEncodingMismatch => "re-export or split the SQL as Unicode text, such as UTF-8 or UTF-16 with a BOM, then retry.",
            MetaTransformScriptSqlImportFailureKind.InvalidSqlInput => "apply target rules and retry: CREATE VIEW/bare SELECT requires --target; inline CREATE FUNCTION forbids --target.",
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

    private static string DisplayTarget(string? targetSqlIdentifier)
    {
        return string.IsNullOrWhiteSpace(targetSqlIdentifier)
            ? "<none>"
            : targetSqlIdentifier;
    }
}
