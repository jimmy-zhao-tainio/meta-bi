using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaConvert.AnalyticsToMultiDimensional;
using MetaConvert.AnalyticsToTabular;
using MetaConvert.DataQualityToSql;
using MetaConvert.DataVaultToSql;
using MetaConvert.DataWarehouseToSql;
using MetaConvert.SchemaToDataVault;
using MetaConvert.SqlToTransformScript;
using MetaConvert.TransformScriptToSql;
using MetaRawDataVault.Instance;
using MetaSchema.Instance;

internal static class Program
{
    private const string AppName = "meta-convert";

    private static readonly ConsolePresenter Presenter = new();
    private static readonly IReadOnlyList<CliCommandRoute> CommandRoutes = BuildCommandRoutes();
    private static readonly IReadOnlyDictionary<string, CliCommandRoute> CommandRoutesByName = CommandRoutes
        .ToDictionary(route => route.Definition.Name, StringComparer.OrdinalIgnoreCase);
    private static readonly CliAppDefinition Cli = new(
        AppName,
        new[]
        {
            "meta-convert <command> [options]"
        },
        CommandRoutes.Select(route => route.Definition).ToArray(),
        Next: "meta-convert schema-to-raw-datavault --help");

    internal static CliAppDefinition CreateAppDefinition() => Cli;

    private static IReadOnlyList<CliCommandRoute> BuildCommandRoutes() =>
        new[]
        {
            new CliCommandRoute(
                new CliCommandDefinition(
                    "help",
                    "Show this help.",
                    new[] { "meta-convert help" }),
                _ =>
                {
                    PrintHelp();
                    return Task.FromResult(0);
                }),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "schema-to-raw-datavault",
                    "Convert MetaSchema workspace to MetaRawDataVault workspace.",
                    new[] { "meta-convert schema-to-raw-datavault --source-workspace <path> --new-workspace <path> [--ignore-field-name <name>]... [--ignore-field-suffix <suffix>]... [--include-views] [--verbose]" },
                    new[]
                    {
                        new CliOptionDefinition("--source-workspace <path>", "Required. MetaSchema workspace to convert."),
                        new CliOptionDefinition("--new-workspace <path>", "Required. Directory where the MetaRawDataVault workspace will be created."),
                        new CliOptionDefinition("--ignore-field-name <name>", "Optional source field name to ignore. May be repeated."),
                        new CliOptionDefinition("--ignore-field-suffix <suffix>", "Optional source field suffix to ignore. May be repeated."),
                        new CliOptionDefinition("--include-views", "Optional. Include source views."),
                        new CliOptionDefinition("--verbose", "Optional. Print conversion summary.")
                    },
                    new[]
                    {
                        "Loads MetaSchema from --source-workspace and saves MetaRawDataVault at --new-workspace.",
                        "Uses typed MetaSchema and MetaRawDataVault instance/tooling libraries.",
                        "Does not use generic workspace model loading."
                    }),
                RunSchemaToRawDataVaultAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "raw-datavault-to-sql",
                    "Convert MetaRawDataVault workspace to MetaSql workspace.",
                    new[] { "meta-convert raw-datavault-to-sql [--workspace <path>] --implementation-workspace <path> --database-name <name> --out <path>" },
                    DataVaultToSqlOptions(),
                    new[]
                    {
                        "Converts the current sanctioned MetaRawDataVault workspace to a current MetaSql workspace.",
                        "Target schema comes from the sanctioned MetaDataVaultImplementation workspace.",
                        "Does not query any live database.",
                        "Saves the generated current MetaSql workspace at --out.",
                        "Defaults to the current working directory when --workspace is omitted."
                    }),
                RunRawDataVaultToSqlAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "business-datavault-to-sql",
                    "Convert MetaBusinessDataVault workspace to MetaSql workspace.",
                    new[] { "meta-convert business-datavault-to-sql [--workspace <path>] --implementation-workspace <path> --database-name <name> --out <path>" },
                    DataVaultToSqlOptions(),
                    new[]
                    {
                        "Converts the current sanctioned MetaBusinessDataVault workspace to a current MetaSql workspace.",
                        "Applies sanctioned business-type lowering during conversion.",
                        "Target schema comes from the sanctioned MetaDataVaultImplementation workspace.",
                        "Does not query any live database.",
                        "Saves the generated current MetaSql workspace at --out.",
                        "Defaults to the current working directory when --workspace is omitted."
                    }),
                RunBusinessDataVaultToSqlAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "data-quality-to-sql",
                    "Convert promoted MetaDataQuality candidates to SQL DQ views.",
                    new[] { "meta-convert data-quality-to-sql [--workspace <path>] --out <path>" },
                    WorkspaceOutOptions("MetaDataQuality workspace to convert."),
                    new[]
                    {
                        "Reads promoted candidates from a MetaDataQuality workspace.",
                        "Generates SQL view scripts plus a MetaDQ operational pack (run/finding tables and execution procedure).",
                        "The operational procedure reads dq.v_DataQualityReview from a source database and persists each run in MetaDQ.",
                        "Defaults to the current working directory when --workspace is omitted."
                    }),
                RunDataQualityToSqlAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "data-warehouse-to-sql",
                    "Convert MetaDataWarehouse workspace to MetaSql workspace.",
                    new[] { "meta-convert data-warehouse-to-sql [--workspace <path>] --implementation-workspace <path> --database-name <name> --out <path>" },
                    DataVaultToSqlOptions(),
                    new[]
                    {
                        "Converts the current sanctioned MetaDataWarehouse workspace to a current MetaSql workspace.",
                        "Target table/column/key policy comes from the sanctioned MetaDataWarehouseImplementation workspace.",
                        "Does not query any live database.",
                        "Saves the generated current MetaSql workspace at --out.",
                        "Defaults to the current working directory when --workspace is omitted."
                    }),
                RunDataWarehouseToSqlAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "transform-script-to-sql",
                    "Convert MetaTransformScript SQL modules to MetaSql workspace.",
                    new[] { "meta-convert transform-script-to-sql [--workspace <path>] --database-name <name> --out <path>" },
                    TransformScriptToSqlOptions(),
                    new[]
                    {
                        "Converts MetaTransformScript view, function, and stored procedure modules to a current MetaSql workspace.",
                        "SQL module declarations must already be schema-qualified in the source workspace.",
                        "Saves the generated current MetaSql workspace at --out.",
                        "Defaults to the current working directory when --workspace is omitted."
                    }),
                RunTransformScriptToSqlAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "sql-to-transform-script",
                    "Convert MetaSql SQL modules to MetaTransformScript workspace.",
                    new[] { "meta-convert sql-to-transform-script [--workspace <path>] --out <path> [--include-views] [--include-functions] [--include-stored-procedures] [--allow-empty]" },
                    SqlToTransformScriptOptions(),
                    new[]
                    {
                        "Reads view, function, and stored procedure module definitions from a MetaSql workspace.",
                        "Imports each module through the MetaTransformScript SQL importer.",
                        "If any include switch is provided, only selected module kinds are converted.",
                        "Saves the generated current MetaTransformScript workspace at --out.",
                        "Defaults to the current working directory when --workspace is omitted."
                    }),
                RunSqlToTransformScriptAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "analytics-to-tabular",
                    "Convert MetaAnalytics workspace to MetaTabular workspace.",
                    new[] { "meta-convert analytics-to-tabular [--workspace <path>] --out <path>" },
                    WorkspaceOutOptions("MetaAnalytics workspace to convert."),
                    new[]
                    {
                        "Converts common MetaAnalytics intent to a MetaTabular workspace.",
                        "DAX expressions are copied when present; non-DAX expressions fail clearly.",
                        "Target-specific calculation groups, partitions, and deployment details are patched in MetaTabular after conversion.",
                        "Defaults to the current working directory when --workspace is omitted."
                    }),
                RunAnalyticsToTabularAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "analytics-to-multi-dimensional",
                    "Convert MetaAnalytics workspace to MetaMultiDimensional workspace.",
                    new[] { "meta-convert analytics-to-multi-dimensional [--workspace <path>] --out <path>" },
                    WorkspaceOutOptions("MetaAnalytics workspace to convert."),
                    new[]
                    {
                        "Converts common MetaAnalytics intent to a MetaMultiDimensional workspace.",
                        "Tabular-style row/object security and measure-expression rows fail clearly until multidimensional calculated-measure projection is modeled.",
                        "Target-specific measure groups, cell security, named sets, actions, partitions, and deployment details are patched in MetaMultiDimensional after conversion.",
                        "Defaults to the current working directory when --workspace is omitted."
                    }),
                RunAnalyticsToMultiDimensionalAsync)
        };

    private static IReadOnlyList<CliOptionDefinition> DataVaultToSqlOptions() =>
        new[]
        {
            new CliOptionDefinition("--workspace <path>", "Optional. Source workspace path. Default: current working directory."),
            new CliOptionDefinition("--implementation-workspace <path>", "Required. Implementation policy workspace."),
            new CliOptionDefinition("--database-name <name>", "Required. Target MetaSql database name."),
            new CliOptionDefinition("--out <path>", "Required. Output MetaSql workspace path.")
        };

    private static IReadOnlyList<CliOptionDefinition> WorkspaceOutOptions(string workspaceDescription) =>
        new[]
        {
            new CliOptionDefinition("--workspace <path>", $"Optional. {workspaceDescription} Default: current working directory."),
            new CliOptionDefinition("--out <path>", "Required. Output workspace or script folder path.")
        };

    private static IReadOnlyList<CliOptionDefinition> TransformScriptToSqlOptions() =>
        new[]
        {
            new CliOptionDefinition("--workspace <path>", "Optional. Source MetaTransformScript workspace path. Default: current working directory."),
            new CliOptionDefinition("--database-name <name>", "Required. Target MetaSql database name."),
            new CliOptionDefinition("--out <path>", "Required. Output MetaSql workspace path.")
        };

    private static IReadOnlyList<CliOptionDefinition> SqlToTransformScriptOptions() =>
        new[]
        {
            new CliOptionDefinition("--workspace <path>", "Optional. Source MetaSql workspace path. Default: current working directory."),
            new CliOptionDefinition("--out <path>", "Required. Output MetaTransformScript workspace path."),
            new CliOptionDefinition("--include-views", "Convert view modules. If no include switch is provided, all module kinds are selected."),
            new CliOptionDefinition("--include-functions", "Convert function modules. If no include switch is provided, all module kinds are selected."),
            new CliOptionDefinition("--include-stored-procedures", "Convert stored procedure modules. If no include switch is provided, all module kinds are selected."),
            new CliOptionDefinition("--allow-empty", "Create an empty MetaTransformScript workspace when selected module kinds have no convertible modules.")
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

        return Fail($"unknown command '{args[0]}'.", $"{AppName} help");
    }

    private static async Task<int> RunSchemaToRawDataVaultAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintSchemaToRawDataVaultHelp();
            return 0;
        }

        var parse = ParseSchemaToRawDataVaultArgs(args, startIndex: 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("schema-to-raw-datavault"));
        }

        var sourceWorkspacePath = Path.GetFullPath(parse.SourceWorkspacePath);
        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parse.NewWorkspacePath);
        if (!targetValidation.Ok)
        {
            return Fail(targetValidation.ErrorMessage, "choose a new folder or empty the target directory and retry.", 4, targetValidation.Details);
        }

        var targetWorkspacePath = targetValidation.FullPath;
        Directory.CreateDirectory(targetWorkspacePath);

        RawDataVaultFromMetaSchemaService.RawDataVaultFromMetaSchemaResult result;
        try
        {
            var sourceModel = await MetaSchemaInstance.LoadFromWorkspaceAsync(
                sourceWorkspacePath,
                searchUpward: false).ConfigureAwait(false);

            result = new RawDataVaultFromMetaSchemaService().MaterializeWithReport(
                sourceModel,
                parse.IgnoreFieldNames,
                parse.IgnoreFieldSuffixes,
                parse.IncludeViews);

            await MetaRawDataVaultInstance.SaveToWorkspaceAsync(
                result.Model,
                targetWorkspacePath).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert schema to raw DataVault.",
                "check source workspace, output path, and options, then retry.",
                4,
                new[]
                {
                    $"  SourceWorkspace: {sourceWorkspacePath}",
                    $"  TargetWorkspace: {targetWorkspacePath}",
                    $"  {ex.Message}",
                });
        }

        Presenter.WriteOk($"Created {Path.GetFileName(targetWorkspacePath)}");
        if (parse.Verbose)
        {
            RenderSummary(result.Report.Summary);
        }

        return 0;
    }

    private static async Task<int> RunRawDataVaultToSqlAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintRawDataVaultToSqlHelp();
            return 0;
        }

        var parse = ParseDataVaultToSqlArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("raw-datavault-to-sql"));
        }

        var workspacePath = Path.GetFullPath(parse.WorkspacePath);
        var implementationWorkspacePath = Path.GetFullPath(parse.ImplementationWorkspacePath);
        var outputWorkspacePath = Path.GetFullPath(parse.OutputPath);

        try
        {
            Directory.CreateDirectory(outputWorkspacePath);
            await Converter.ConvertAsync(
                workspacePath,
                outputWorkspacePath,
                implementationWorkspacePath,
                parse.DatabaseName).ConfigureAwait(false);

            Presenter.WriteOk($"Generated {Path.GetFileName(outputWorkspacePath)}");
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert raw DataVault to SQL.",
                "check the workspace, implementation workspace, and database name, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Database: {parse.DatabaseName}",
                    $"  Output: {outputWorkspacePath}",
                    $"  {ex.Message}",
                });
        }
    }

    private static async Task<int> RunBusinessDataVaultToSqlAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintBusinessDataVaultToSqlHelp();
            return 0;
        }

        var parse = ParseDataVaultToSqlArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("business-datavault-to-sql"));
        }

        var workspacePath = Path.GetFullPath(parse.WorkspacePath);
        var implementationWorkspacePath = Path.GetFullPath(parse.ImplementationWorkspacePath);
        var outputWorkspacePath = Path.GetFullPath(parse.OutputPath);

        try
        {
            Directory.CreateDirectory(outputWorkspacePath);
            await Converter.ConvertAsync(
                workspacePath,
                outputWorkspacePath,
                implementationWorkspacePath,
                parse.DatabaseName).ConfigureAwait(false);

            Presenter.WriteOk($"Generated {Path.GetFileName(outputWorkspacePath)}");
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert business DataVault to SQL.",
                "check the workspace, implementation workspace, and database name, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Database: {parse.DatabaseName}",
                    $"  Output: {outputWorkspacePath}",
                    $"  {ex.Message}",
                });
        }
    }

    private static Task<int> RunDataQualityToSqlAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintDataQualityToSqlHelp();
            return Task.FromResult(0);
        }

        var parse = ParseDataQualityToSqlArgs(args, 1);
        if (!parse.Ok)
        {
            return Task.FromResult(Fail(parse.ErrorMessage, HelpCommand("data-quality-to-sql")));
        }

        var workspacePath = Path.GetFullPath(parse.WorkspacePath);
        var outputPath = Path.GetFullPath(parse.OutputPath);

        try
        {
            var result = new DataQualityToSqlConverter().Convert(workspacePath, outputPath);
            Presenter.WriteInfo(
                $"Generated {result.CandidateViewCount} data quality view script{(result.CandidateViewCount == 1 ? string.Empty : "s")}, " +
                $"{result.DashboardViewCount} review dashboard, and MetaDQ operational SQL ({result.OperationalTableCount} tables, {result.OperationalProcedureCount} procedure).");
            Presenter.WriteKeyValueBlock("Output", new[]
            {
                ("DataQualityViews", result.CandidateViewCount.ToString()),
                ("Dashboard", "dq.v_DataQualityReview"),
                ("MetaDQTables", result.OperationalTableCount.ToString()),
                ("MetaDQProcedures", result.OperationalProcedureCount.ToString()),
                ("Scripts", result.ScriptCount.ToString()),
                ("Path", result.OutputPath),
            });
            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            return Task.FromResult(Fail(
                "Cannot convert data-quality workspace to SQL.",
                "check the workspace, promoted candidates, and output path, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Output: {outputPath}",
                    $"  {ex.Message}",
                }));
        }
    }

    private static async Task<int> RunDataWarehouseToSqlAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintDataWarehouseToSqlHelp();
            return 0;
        }

        var parse = ParseDataVaultToSqlArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("data-warehouse-to-sql"));
        }

        var workspacePath = Path.GetFullPath(parse.WorkspacePath);
        var implementationWorkspacePath = Path.GetFullPath(parse.ImplementationWorkspacePath);
        var outputWorkspacePath = Path.GetFullPath(parse.OutputPath);

        try
        {
            Directory.CreateDirectory(outputWorkspacePath);
            await DataWarehouseToSqlConverter.ConvertAsync(
                workspacePath,
                outputWorkspacePath,
                implementationWorkspacePath,
                parse.DatabaseName).ConfigureAwait(false);

            Presenter.WriteOk($"Generated {Path.GetFileName(outputWorkspacePath)}");
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert data warehouse to SQL.",
                "check the workspace, implementation workspace, and database name, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Database: {parse.DatabaseName}",
                    $"  Output: {outputWorkspacePath}",
                    $"  {ex.Message}",
                });
        }
    }

    private static async Task<int> RunTransformScriptToSqlAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintTransformScriptToSqlHelp();
            return 0;
        }

        var parse = ParseTransformScriptToSqlArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("transform-script-to-sql"));
        }

        var workspacePath = Path.GetFullPath(parse.WorkspacePath);
        var outputWorkspacePath = Path.GetFullPath(parse.OutputPath);

        try
        {
            Directory.CreateDirectory(outputWorkspacePath);
            await TransformScriptToSqlConverter.ConvertAsync(
                workspacePath,
                outputWorkspacePath,
                parse.DatabaseName).ConfigureAwait(false);

            Presenter.WriteInfo($"Generated {Path.GetFileName(outputWorkspacePath)}");
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert transform scripts to SQL.",
                "check the workspace, database name, schema-qualified module declarations, and output path, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Database: {parse.DatabaseName}",
                    $"  Output: {outputWorkspacePath}",
                    $"  {ex.Message}",
            });
        }
    }

    private static async Task<int> RunSqlToTransformScriptAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintSqlToTransformScriptHelp();
            return 0;
        }

        var parse = ParseSqlToTransformScriptArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("sql-to-transform-script"));
        }

        var workspacePath = Path.GetFullPath(parse.WorkspacePath);
        var outputWorkspacePath = Path.GetFullPath(parse.OutputPath);

        try
        {
            var result = await SqlToTransformScriptConverter.ConvertAsync(
                workspacePath,
                outputWorkspacePath,
                new SqlToTransformScriptConversionOptions
                {
                    ModuleKinds = parse.ModuleKinds,
                    AllowEmpty = parse.AllowEmpty,
                }).ConfigureAwait(false);

            Presenter.WriteInfo($"Generated {Path.GetFileName(outputWorkspacePath)}");
            Presenter.WriteKeyValueBlock("Summary", new[]
            {
                ("Views", result.ViewCount.ToString()),
                ("Functions", result.FunctionCount.ToString()),
                ("StoredProcedures", result.StoredProcedureCount.ToString()),
            });
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert MetaSql to transform scripts.",
                "check the MetaSql workspace, module definitions, and output path, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Output: {outputWorkspacePath}",
                    $"  {ex.Message}",
                });
        }
    }

    private static async Task<int> RunAnalyticsToTabularAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintAnalyticsToTabularHelp();
            return 0;
        }

        var parse = ParseDataQualityToSqlArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("analytics-to-tabular"));
        }

        var workspacePath = Path.GetFullPath(parse.WorkspacePath);
        var outputPath = Path.GetFullPath(parse.OutputPath);

        try
        {
            Directory.CreateDirectory(outputPath);
            var result = await AnalyticsToTabularConverter.ConvertAsync(workspacePath, outputPath).ConfigureAwait(false);
            Presenter.WriteOk($"Generated {Path.GetFileName(outputPath)}");
            Presenter.WriteKeyValueBlock("Summary", new[]
            {
                ("Tables", result.TableCount.ToString()),
                ("Columns", result.ColumnCount.ToString()),
                ("Measures", result.MeasureCount.ToString()),
            });
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert analytics to tabular.",
                "check the workspace, expression languages, and output path, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Output: {outputPath}",
                    $"  {ex.Message}",
                });
        }
    }

    private static async Task<int> RunAnalyticsToMultiDimensionalAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintAnalyticsToMultiDimensionalHelp();
            return 0;
        }

        var parse = ParseDataQualityToSqlArgs(args, 1);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("analytics-to-multi-dimensional"));
        }

        var workspacePath = Path.GetFullPath(parse.WorkspacePath);
        var outputPath = Path.GetFullPath(parse.OutputPath);

        try
        {
            Directory.CreateDirectory(outputPath);
            var result = await AnalyticsToMultiDimensionalConverter.ConvertAsync(workspacePath, outputPath).ConfigureAwait(false);
            Presenter.WriteOk($"Generated {Path.GetFileName(outputPath)}");
            Presenter.WriteKeyValueBlock("Summary", new[]
            {
                ("Cubes", result.CubeCount.ToString()),
                ("Dimensions", result.DimensionCount.ToString()),
                ("MeasureGroups", result.MeasureGroupCount.ToString()),
                ("Measures", result.MeasureCount.ToString()),
            });
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot convert analytics to multidimensional.",
                "check the workspace, target-specific expressions/security, and output path, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  Output: {outputPath}",
                    $"  {ex.Message}",
                });
        }
    }

    private static (bool Ok, string SourceWorkspacePath, string NewWorkspacePath, List<string> IgnoreFieldNames, List<string> IgnoreFieldSuffixes, bool IncludeViews, bool Verbose, string ErrorMessage) ParseSchemaToRawDataVaultArgs(
        string[] args,
        int startIndex)
    {
        var sourceWorkspacePath = string.Empty;
        var newWorkspacePath = string.Empty;
        var ignoreFieldNames = new List<string>();
        var ignoreFieldSuffixes = new List<string>();
        var includeViews = false;
        var verbose = false;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--source-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --source-workspace.");
                if (!string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "--source-workspace can only be provided once.");
                sourceWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "--new-workspace can only be provided once.");
                newWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--ignore-field-name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --ignore-field-name.");
                ignoreFieldNames.Add(args[++i]);
                continue;
            }

            if (string.Equals(arg, "--ignore-field-suffix", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing value for --ignore-field-suffix.");
                ignoreFieldSuffixes.Add(args[++i]);
                continue;
            }

            if (string.Equals(arg, "--include-views", StringComparison.OrdinalIgnoreCase))
            {
                includeViews = true;
                continue;
            }

            if (string.Equals(arg, "--verbose", StringComparison.OrdinalIgnoreCase))
            {
                verbose = true;
                continue;
            }

            return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing required option --source-workspace <path>.");
        if (string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, "missing required option --new-workspace <path>.");

        return (true, sourceWorkspacePath, newWorkspacePath, ignoreFieldNames, ignoreFieldSuffixes, includeViews, verbose, string.Empty);
    }

    private static (bool Ok, string WorkspacePath, string ImplementationWorkspacePath, string OutputPath, string DatabaseName, string ErrorMessage) ParseDataVaultToSqlArgs(string[] args, int startIndex)
    {
        var workspacePath = ".";
        var workspaceSpecified = false;
        var implementationWorkspacePath = string.Empty;
        var outputPath = string.Empty;
        var databaseName = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing value for --workspace.");
                if (workspaceSpecified) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "--workspace can only be provided once.");
                workspacePath = args[++i];
                workspaceSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--implementation-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing value for --implementation-workspace.");
                if (!string.IsNullOrWhiteSpace(implementationWorkspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "--implementation-workspace can only be provided once.");
                implementationWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--out", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing value for --out.");
                if (!string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "--out can only be provided once.");
                outputPath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--database-name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing value for --database-name.");
                if (!string.IsNullOrWhiteSpace(databaseName)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "--database-name can only be provided once.");
                databaseName = args[++i];
                continue;
            }

            return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(implementationWorkspacePath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing required option --implementation-workspace <path>.");
        if (string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing required option --out <path>.");
        if (string.IsNullOrWhiteSpace(databaseName)) return (false, workspacePath, implementationWorkspacePath, outputPath, databaseName, "missing required option --database-name <name>.");
        return (true, workspacePath, implementationWorkspacePath, outputPath, databaseName, string.Empty);
    }

    private static (bool Ok, string WorkspacePath, string OutputPath, string DatabaseName, string ErrorMessage) ParseTransformScriptToSqlArgs(
        string[] args,
        int startIndex)
    {
        var workspacePath = ".";
        var workspaceSpecified = false;
        var outputPath = string.Empty;
        var databaseName = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, outputPath, databaseName, "missing value for --workspace.");
                if (workspaceSpecified) return (false, workspacePath, outputPath, databaseName, "--workspace can only be provided once.");
                workspacePath = args[++i];
                workspaceSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--out", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, outputPath, databaseName, "missing value for --out.");
                if (!string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, outputPath, databaseName, "--out can only be provided once.");
                outputPath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--database-name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, outputPath, databaseName, "missing value for --database-name.");
                if (!string.IsNullOrWhiteSpace(databaseName)) return (false, workspacePath, outputPath, databaseName, "--database-name can only be provided once.");
                databaseName = args[++i];
                continue;
            }

            return (false, workspacePath, outputPath, databaseName, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, outputPath, databaseName, "missing required option --out <path>.");
        if (string.IsNullOrWhiteSpace(databaseName)) return (false, workspacePath, outputPath, databaseName, "missing required option --database-name <name>.");
        return (true, workspacePath, outputPath, databaseName, string.Empty);
    }

    private static (bool Ok, string WorkspacePath, string OutputPath, string ErrorMessage) ParseDataQualityToSqlArgs(string[] args, int startIndex)
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

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            return (false, workspacePath, outputPath, "missing required option --out <path>.");
        }

        return (true, workspacePath, outputPath, string.Empty);
    }

    private static (bool Ok, string WorkspacePath, string OutputPath, SqlToTransformScriptModuleKinds ModuleKinds, bool AllowEmpty, string ErrorMessage) ParseSqlToTransformScriptArgs(
        string[] args,
        int startIndex)
    {
        var workspacePath = ".";
        var workspaceSpecified = false;
        var outputPath = string.Empty;
        var includeSwitchProvided = false;
        var moduleKinds = SqlToTransformScriptModuleKinds.All;
        var allowEmpty = false;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, outputPath, moduleKinds, allowEmpty, "missing value for --workspace.");
                if (workspaceSpecified) return (false, workspacePath, outputPath, moduleKinds, allowEmpty, "--workspace can only be provided once.");
                workspacePath = args[++i];
                workspaceSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--out", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, outputPath, moduleKinds, allowEmpty, "missing value for --out.");
                if (!string.IsNullOrWhiteSpace(outputPath)) return (false, workspacePath, outputPath, moduleKinds, allowEmpty, "--out can only be provided once.");
                outputPath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--include-views", StringComparison.OrdinalIgnoreCase))
            {
                AddModuleKindFilter(ref includeSwitchProvided, ref moduleKinds, SqlToTransformScriptModuleKinds.Views);
                continue;
            }

            if (string.Equals(arg, "--include-functions", StringComparison.OrdinalIgnoreCase))
            {
                AddModuleKindFilter(ref includeSwitchProvided, ref moduleKinds, SqlToTransformScriptModuleKinds.Functions);
                continue;
            }

            if (string.Equals(arg, "--include-stored-procedures", StringComparison.OrdinalIgnoreCase))
            {
                AddModuleKindFilter(ref includeSwitchProvided, ref moduleKinds, SqlToTransformScriptModuleKinds.StoredProcedures);
                continue;
            }

            if (string.Equals(arg, "--allow-empty", StringComparison.OrdinalIgnoreCase))
            {
                allowEmpty = true;
                continue;
            }

            return (false, workspacePath, outputPath, moduleKinds, allowEmpty, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            return (false, workspacePath, outputPath, moduleKinds, allowEmpty, "missing required option --out <path>.");
        }

        return (true, workspacePath, outputPath, moduleKinds, allowEmpty, string.Empty);
    }

    private static void AddModuleKindFilter(
        ref bool includeSwitchProvided,
        ref SqlToTransformScriptModuleKinds moduleKinds,
        SqlToTransformScriptModuleKinds moduleKind)
    {
        if (!includeSwitchProvided)
        {
            moduleKinds = SqlToTransformScriptModuleKinds.None;
            includeSwitchProvided = true;
        }

        moduleKinds |= moduleKind;
    }

    private static void RenderSummary(RawDataVaultFromMetaSchemaSummary summary)
    {
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteKeyValueBlock("Summary", new[]
        {
            ("Source systems", summary.SourceSystemCount.ToString()),
            ("Source schemas", summary.SourceSchemaCount.ToString()),
            ("Source tables", summary.SourceTableCount.ToString()),
            ("Source relationships", summary.SourceRelationshipCount.ToString()),
            ("Raw hubs", summary.RawHubCount.ToString()),
            ("Raw hub key parts", summary.RawHubKeyPartCount.ToString()),
            ("Raw links", summary.RawLinkCount.ToString()),
            ("Raw hub satellites", summary.RawHubSatelliteCount.ToString()),
            ("Raw hub satellite attributes", summary.RawHubSatelliteAttributeCount.ToString()),
            ("Ignored field names", FormatSummaryList(summary.IgnoredFieldNames)),
            ("Ignored field suffixes", FormatSummaryList(summary.IgnoredFieldSuffixes)),
            ("Included views", summary.IncludeViews ? "yes" : "no"),
        });
    }

    private static string FormatSummaryList(IEnumerable<string> values)
    {
        var materialized = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ThenBy(value => value, StringComparer.Ordinal)
            .ToList();

        return materialized.Count == 0
            ? "(none)"
            : string.Join(", ", materialized);
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

    private static void PrintSchemaToRawDataVaultHelp()
    {
        PrintCommandHelp("schema-to-raw-datavault");
    }

    private static void PrintRawDataVaultToSqlHelp()
    {
        PrintCommandHelp("raw-datavault-to-sql");
    }

    private static void PrintBusinessDataVaultToSqlHelp()
    {
        PrintCommandHelp("business-datavault-to-sql");
    }

    private static void PrintDataQualityToSqlHelp()
    {
        PrintCommandHelp("data-quality-to-sql");
    }

    private static void PrintDataWarehouseToSqlHelp()
    {
        PrintCommandHelp("data-warehouse-to-sql");
    }

    private static void PrintTransformScriptToSqlHelp()
    {
        PrintCommandHelp("transform-script-to-sql");
    }

    private static void PrintSqlToTransformScriptHelp()
    {
        PrintCommandHelp("sql-to-transform-script");
    }

    private static void PrintAnalyticsToTabularHelp()
    {
        PrintCommandHelp("analytics-to-tabular");
    }

    private static void PrintAnalyticsToMultiDimensionalHelp()
    {
        PrintCommandHelp("analytics-to-multi-dimensional");
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
