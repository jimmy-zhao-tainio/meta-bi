using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using Meta.Core.Connections;
using MetaSql;
using MetaSql.Extractors.SqlServer;
using MetaSqlDeployManifest;
using System.Text.Json;

internal static partial class Program
{
    private const string AppName = "meta-sql";

    private static readonly ConsolePresenter Presenter = new();
    private static readonly IReadOnlyList<CliCommandRoute> CommandRoutes = BuildCommandRoutes();
    private static readonly IReadOnlyDictionary<string, CliCommandRoute> CommandRoutesByName = CommandRoutes
        .ToDictionary(route => route.Definition.Name, StringComparer.OrdinalIgnoreCase);
    private static readonly CliAppDefinition Cli = new(
        AppName,
        new[]
        {
            "meta-sql <command> [options]"
        },
        CommandRoutes.Select(route => route.Definition).Concat(new[] { CreateExtractSqlServerCommand() }).ToArray(),
        Next: "meta-sql deploy-plan --help");

    internal static CliAppDefinition CreateAppDefinition() => Cli;

    private static IReadOnlyList<CliCommandRoute> BuildCommandRoutes() =>
        new[]
        {
            new CliCommandRoute(
                new CliCommandDefinition(
                    "help",
                    "Show this help.",
                    new[] { "meta-sql help" }),
                _ =>
                {
                    PrintHelp();
                    return Task.FromResult(0);
                }),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "extract",
                    "Materialize sanctioned MetaSql workspaces from external sources.",
                    new[] { "meta-sql extract <extractor> [options]" },
                    Notes: new[] { "Available extractor: sqlserver." },
                    Next: "meta-sql extract sqlserver --help"),
                RunExtractAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "deploy-plan",
                    "Create a deploy manifest (add/alter/block/replace; destructive actions require exact object-scoped approvals).",
                    new[] { "meta-sql deploy-plan --source-workspace <path> --connection-env <name> --out <path> [--approve-drop-table <schema.table>] [--approve-drop-column <schema.table.column>] [--approve-truncate-column <schema.table.column>] [--approval-file <path>]" },
                    new[]
                    {
                        new CliOptionDefinition("--source-workspace <path>", "Required. Source MetaSql workspace to compare with the live database."),
                        new CliOptionDefinition("--connection-env <name>", "Required. Environment variable containing the SQL Server connection string."),
                        new CliOptionDefinition("--out <path>", "Required. Directory where the deploy manifest workspace will be written."),
                        new CliOptionDefinition("--approve-drop-table <schema.table>", "Approve one exact destructive table drop."),
                        new CliOptionDefinition("--approve-drop-column <schema.table.column>", "Approve one exact destructive column drop."),
                        new CliOptionDefinition("--approve-truncate-column <schema.table.column>", "Approve one exact destructive column truncation."),
                        new CliOptionDefinition("--approval-file <path>", "Optional JSON file containing destructive approvals.")
                    },
                    new[]
                    {
                        "Loads the source MetaSql workspace.",
                        "Extracts the live SQL Server schema to MetaSql.",
                        "Always plans against the full source workspace and full live database. Filtered subset deploy is not supported.",
                        "Creates a deploy manifest with Add/Drop/Truncate/Alter/Replace/Block entries.",
                        "DataDropTable and DataDropColumn require exact object-scoped approvals.",
                        "DataTruncationColumn requires exact object-scoped approval.",
                        "Approvals can be passed as repeated CLI arguments and/or via --approval-file JSON.",
                        "Live-only DropPrimaryKey/DropForeignKey/DropIndex are planned by default.",
                        "Shared table-column differences become AlterTableColumn when executable and feasible.",
                        "Shared primary-key differences become ReplacePrimaryKey when executable; otherwise they are blocked.",
                        "Shared foreign-key differences become ReplaceForeignKey when executable; otherwise they are blocked.",
                        "Shared index differences become ReplaceIndex when executable; otherwise they are blocked.",
                        "Deployable only when there are no block entries."
                    }),
                RunDeployPlanAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "deploy",
                    "Apply a deploy manifest after source/live fingerprint validation.",
                    new[] { "meta-sql deploy --manifest-workspace <path> --source-workspace <path> --connection-env <name>" },
                    new[]
                    {
                        new CliOptionDefinition("--manifest-workspace <path>", "Required. Deploy manifest workspace created by deploy-plan."),
                        new CliOptionDefinition("--source-workspace <path>", "Required. Source MetaSql workspace used to create the manifest."),
                        new CliOptionDefinition("--connection-env <name>", "Required. Environment variable containing the SQL Server connection string.")
                    },
                    new[]
                    {
                        "Loads the deploy manifest and source MetaSql workspace.",
                        "Refuses when the manifest contains Block entries.",
                        "Refuses when source/live instance fingerprints no longer match.",
                        "Always validates and applies the full manifest scope. Filtered subset deploy is not supported.",
                        "Creates the database first when the manifest expects a missing database.",
                        "Executes deploy statements without wrapping the full deploy in one SQL transaction.",
                        "If later statements fail after database creation, the database remains and the failure reports that explicitly."
                    }),
                RunDeployAsync),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "execute",
                    "Execute a SQL Server file or query for demo/bootstrap/verification scripts.",
                    new[] { "meta-sql execute --connection-env <name> (--file <path> | --query <sql>) [--var <name=value>] [--timeout-seconds <seconds>] [--quiet]" },
                    new[]
                    {
                        new CliOptionDefinition("--connection-env <name>", "Required. Environment variable containing the SQL Server connection string."),
                        new CliOptionDefinition("--file <path>", "Execute SQL from a file. Mutually exclusive with --query."),
                        new CliOptionDefinition("--query <sql>", "Execute inline SQL. Mutually exclusive with --file."),
                        new CliOptionDefinition("--var <name=value>", "Replace one SQLCMD-style $(NAME) token before execution. Can be repeated."),
                        new CliOptionDefinition("--timeout-seconds <seconds>", "Command timeout for each SQL batch. 0 or omitted means no timeout."),
                        new CliOptionDefinition("--quiet", "Suppress result-set and success output.")
                    },
                    new[]
                    {
                        "Executes SQL Server SQL batches for demo/bootstrap/verification scripts.",
                        "Batch separators use GO lines; --var replaces $(NAME) tokens before execution.",
                        "This command is an execution helper. Metadata realization still belongs to deploy-plan/deploy."
                    }),
                RunExecuteAsync)
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

    private static (bool Ok, string SourceWorkspacePath, string OutputPath, string ConnectionEnvironmentVariableName, IReadOnlyList<MetaSqlDestructiveApproval> DestructiveApprovals, string ErrorMessage) ParseDiffLikeArgs(string[] args, int startIndex)
    {
        var sourceWorkspacePath = string.Empty;
        var outputPath = string.Empty;
        var connectionEnvironmentVariableName = string.Empty;
        string? approvalFilePath = null;
        var explicitApprovals = new List<MetaSqlDestructiveApproval>();

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--source-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, "missing value for --source-workspace.");
                if (!string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, "--source-workspace can only be provided once.");
                sourceWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--out", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, "missing value for --out.");
                if (!string.IsNullOrWhiteSpace(outputPath)) return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, "--out can only be provided once.");
                outputPath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, "missing value for --connection-env.");
                if (!string.IsNullOrWhiteSpace(connectionEnvironmentVariableName)) return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, "--connection-env can only be provided once.");
                connectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--approve-drop-table", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, "missing value for --approve-drop-table.");
                var value = args[++i];
                if (!TryParseTableScope(value, out var approvedSchema, out var approvedTable))
                {
                    return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, $"invalid table scope '{value}' for --approve-drop-table. Expected <schema>.<table>.");
                }

                explicitApprovals.Add(new MetaSqlDestructiveApproval
                {
                    Kind = MetaSqlDestructiveApprovalKind.DataDropTable,
                    SchemaName = approvedSchema,
                    TableName = approvedTable,
                });
                continue;
            }

            if (string.Equals(arg, "--approve-drop-column", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, "missing value for --approve-drop-column.");
                var value = args[++i];
                if (!TryParseColumnScope(value, out var approvedSchema, out var approvedTable, out var approvedColumn))
                {
                    return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, $"invalid column scope '{value}' for --approve-drop-column. Expected <schema>.<table>.<column>.");
                }

                explicitApprovals.Add(new MetaSqlDestructiveApproval
                {
                    Kind = MetaSqlDestructiveApprovalKind.DataDropColumn,
                    SchemaName = approvedSchema,
                    TableName = approvedTable,
                    ColumnName = approvedColumn,
                });
                continue;
            }

            if (string.Equals(arg, "--approve-truncate-column", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, "missing value for --approve-truncate-column.");
                var value = args[++i];
                if (!TryParseColumnScope(value, out var approvedSchema, out var approvedTable, out var approvedColumn))
                {
                    return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, $"invalid column scope '{value}' for --approve-truncate-column. Expected <schema>.<table>.<column>.");
                }

                explicitApprovals.Add(new MetaSqlDestructiveApproval
                {
                    Kind = MetaSqlDestructiveApprovalKind.DataTruncationColumn,
                    SchemaName = approvedSchema,
                    TableName = approvedTable,
                    ColumnName = approvedColumn,
                });
                continue;
            }

            if (string.Equals(arg, "--approval-file", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, "missing value for --approval-file.");
                if (!string.IsNullOrWhiteSpace(approvalFilePath)) return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, "--approval-file can only be provided once.");
                approvalFilePath = args[++i];
                continue;
            }

            return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, "missing required option --source-workspace <path>.");
        if (string.IsNullOrWhiteSpace(connectionEnvironmentVariableName)) return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, "missing required option --connection-env <name>.");

        if (!string.IsNullOrWhiteSpace(approvalFilePath))
        {
            try
            {
                var fileApprovals = LoadApprovalsFromFile(approvalFilePath);
                explicitApprovals.AddRange(fileApprovals);
            }
            catch (Exception ex)
            {
                return (false, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, ex.Message);
            }
        }

        return (true, sourceWorkspacePath, outputPath, connectionEnvironmentVariableName, explicitApprovals, string.Empty);
    }

    private static (bool Ok, string ManifestWorkspacePath, string SourceWorkspacePath, string ConnectionEnvironmentVariableName, string ErrorMessage) ParseDeployArgs(string[] args, int startIndex)
    {
        var manifestWorkspacePath = string.Empty;
        var sourceWorkspacePath = string.Empty;
        var connectionEnvironmentVariableName = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--manifest-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionEnvironmentVariableName, "missing value for --manifest-workspace.");
                if (!string.IsNullOrWhiteSpace(manifestWorkspacePath)) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionEnvironmentVariableName, "--manifest-workspace can only be provided once.");
                manifestWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--source-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionEnvironmentVariableName, "missing value for --source-workspace.");
                if (!string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionEnvironmentVariableName, "--source-workspace can only be provided once.");
                sourceWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionEnvironmentVariableName, "missing value for --connection-env.");
                if (!string.IsNullOrWhiteSpace(connectionEnvironmentVariableName)) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionEnvironmentVariableName, "--connection-env can only be provided once.");
                connectionEnvironmentVariableName = args[++i];
                continue;
            }

            return (false, manifestWorkspacePath, sourceWorkspacePath, connectionEnvironmentVariableName, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(manifestWorkspacePath)) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionEnvironmentVariableName, "missing required option --manifest-workspace <path>.");
        if (string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionEnvironmentVariableName, "missing required option --source-workspace <path>.");
        if (string.IsNullOrWhiteSpace(connectionEnvironmentVariableName)) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionEnvironmentVariableName, "missing required option --connection-env <name>.");

        return (true, manifestWorkspacePath, sourceWorkspacePath, connectionEnvironmentVariableName, string.Empty);
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

    private static string FormatManifestChangeSummary(MetaSqlDeployManifestModel manifestModel)
    {
        return FormatActionGroups(
            BuildActionGroup(
                total: manifestModel.AddSchemaList.Count +
                       manifestModel.AddTableList.Count +
                       manifestModel.AddTableColumnList.Count +
                       manifestModel.AddPrimaryKeyList.Count +
                       manifestModel.AddForeignKeyList.Count +
                       manifestModel.AddIndexList.Count +
                       manifestModel.AddViewList.Count +
                       manifestModel.AddFunctionList.Count +
                       manifestModel.AddStoredProcedureList.Count,
                actionLabel: "to add",
                (manifestModel.AddSchemaList.Count, "schema", "schemas"),
                (manifestModel.AddTableList.Count, "table", "tables"),
                (manifestModel.AddTableColumnList.Count, "column", "columns"),
                (manifestModel.AddPrimaryKeyList.Count, "primary key", "primary keys"),
                (manifestModel.AddForeignKeyList.Count, "foreign key", "foreign keys"),
                (manifestModel.AddIndexList.Count, "index", "indexes"),
                (manifestModel.AddViewList.Count, "view", "views"),
                (manifestModel.AddFunctionList.Count, "function", "functions"),
                (manifestModel.AddStoredProcedureList.Count, "stored procedure", "stored procedures")),
            BuildActionGroup(
                total: manifestModel.AlterTableColumnList.Count,
                actionLabel: "to alter",
                (manifestModel.AlterTableColumnList.Count, "column", "columns")),
            BuildActionGroup(
                total: manifestModel.DropTableList.Count +
                       manifestModel.DropTableColumnList.Count +
                       manifestModel.DropPrimaryKeyList.Count +
                       manifestModel.DropForeignKeyList.Count +
                       manifestModel.DropIndexList.Count +
                       manifestModel.DropViewList.Count +
                       manifestModel.DropFunctionList.Count +
                       manifestModel.DropStoredProcedureList.Count,
                actionLabel: "to drop",
                (manifestModel.DropTableList.Count, "table", "tables"),
                (manifestModel.DropTableColumnList.Count, "column", "columns"),
                (manifestModel.DropPrimaryKeyList.Count, "primary key", "primary keys"),
                (manifestModel.DropForeignKeyList.Count, "foreign key", "foreign keys"),
                (manifestModel.DropIndexList.Count, "index", "indexes"),
                (manifestModel.DropViewList.Count, "view", "views"),
                (manifestModel.DropFunctionList.Count, "function", "functions"),
                (manifestModel.DropStoredProcedureList.Count, "stored procedure", "stored procedures")),
            BuildActionGroup(
                total: manifestModel.TruncateTableColumnDataList.Count,
                actionLabel: "to truncate",
                (manifestModel.TruncateTableColumnDataList.Count, "column", "columns")),
            BuildActionGroup(
                total: manifestModel.ReplacePrimaryKeyList.Count +
                       manifestModel.ReplaceForeignKeyList.Count +
                       manifestModel.ReplaceIndexList.Count +
                       manifestModel.ReplaceViewList.Count +
                       manifestModel.ReplaceFunctionList.Count +
                       manifestModel.ReplaceStoredProcedureList.Count,
                actionLabel: "to replace",
                (manifestModel.ReplacePrimaryKeyList.Count, "primary key", "primary keys"),
                (manifestModel.ReplaceForeignKeyList.Count, "foreign key", "foreign keys"),
                (manifestModel.ReplaceIndexList.Count, "index", "indexes"),
                (manifestModel.ReplaceViewList.Count, "view", "views"),
                (manifestModel.ReplaceFunctionList.Count, "function", "functions"),
                (manifestModel.ReplaceStoredProcedureList.Count, "stored procedure", "stored procedures")));
    }

    private static string FormatManifestDeploySummary(MetaSqlDeployManifestModel manifestModel)
    {
        return FormatActionGroups(
            BuildActionGroup(
                total: manifestModel.AddSchemaList.Count +
                       manifestModel.AddTableList.Count +
                       manifestModel.AddTableColumnList.Count +
                       manifestModel.AddPrimaryKeyList.Count +
                       manifestModel.AddForeignKeyList.Count +
                       manifestModel.AddIndexList.Count +
                       manifestModel.AddViewList.Count +
                       manifestModel.AddFunctionList.Count +
                       manifestModel.AddStoredProcedureList.Count,
                actionLabel: "added",
                (manifestModel.AddSchemaList.Count, "schema", "schemas"),
                (manifestModel.AddTableList.Count, "table", "tables"),
                (manifestModel.AddTableColumnList.Count, "column", "columns"),
                (manifestModel.AddPrimaryKeyList.Count, "primary key", "primary keys"),
                (manifestModel.AddForeignKeyList.Count, "foreign key", "foreign keys"),
                (manifestModel.AddIndexList.Count, "index", "indexes"),
                (manifestModel.AddViewList.Count, "view", "views"),
                (manifestModel.AddFunctionList.Count, "function", "functions"),
                (manifestModel.AddStoredProcedureList.Count, "stored procedure", "stored procedures")),
            BuildActionGroup(
                total: manifestModel.AlterTableColumnList.Count,
                actionLabel: "altered",
                (manifestModel.AlterTableColumnList.Count, "column", "columns")),
            BuildActionGroup(
                total: manifestModel.DropTableList.Count +
                       manifestModel.DropTableColumnList.Count +
                       manifestModel.DropPrimaryKeyList.Count +
                       manifestModel.DropForeignKeyList.Count +
                       manifestModel.DropIndexList.Count +
                       manifestModel.DropViewList.Count +
                       manifestModel.DropFunctionList.Count +
                       manifestModel.DropStoredProcedureList.Count,
                actionLabel: "dropped",
                (manifestModel.DropTableList.Count, "table", "tables"),
                (manifestModel.DropTableColumnList.Count, "column", "columns"),
                (manifestModel.DropPrimaryKeyList.Count, "primary key", "primary keys"),
                (manifestModel.DropForeignKeyList.Count, "foreign key", "foreign keys"),
                (manifestModel.DropIndexList.Count, "index", "indexes"),
                (manifestModel.DropViewList.Count, "view", "views"),
                (manifestModel.DropFunctionList.Count, "function", "functions"),
                (manifestModel.DropStoredProcedureList.Count, "stored procedure", "stored procedures")),
            BuildActionGroup(
                total: manifestModel.TruncateTableColumnDataList.Count,
                actionLabel: "truncated",
                (manifestModel.TruncateTableColumnDataList.Count, "column", "columns")),
            BuildActionGroup(
                total: manifestModel.ReplacePrimaryKeyList.Count +
                       manifestModel.ReplaceForeignKeyList.Count +
                       manifestModel.ReplaceIndexList.Count +
                       manifestModel.ReplaceViewList.Count +
                       manifestModel.ReplaceFunctionList.Count +
                       manifestModel.ReplaceStoredProcedureList.Count,
                actionLabel: "replaced",
                (manifestModel.ReplacePrimaryKeyList.Count, "primary key", "primary keys"),
                (manifestModel.ReplaceForeignKeyList.Count, "foreign key", "foreign keys"),
                (manifestModel.ReplaceIndexList.Count, "index", "indexes"),
                (manifestModel.ReplaceViewList.Count, "view", "views"),
                (manifestModel.ReplaceFunctionList.Count, "function", "functions"),
                (manifestModel.ReplaceStoredProcedureList.Count, "stored procedure", "stored procedures")));
    }

    private static string FormatActionGroups(params string[] groups)
    {
        var populatedGroups = groups
            .Where(group => !string.IsNullOrWhiteSpace(group))
            .ToList();

        return populatedGroups.Count == 0
            ? "none"
            : string.Join("; ", populatedGroups);
    }

    private static string BuildActionGroup(
        int total,
        string actionLabel,
        params (int Count, string Singular, string Plural)[] kinds)
    {
        if (total <= 0)
        {
            return string.Empty;
        }

        var kindSummary = kinds
            .Where(kind => kind.Count > 0)
            .Select(kind => FormatCount(kind.Count, kind.Singular, kind.Plural))
            .ToList();

        return kindSummary.Count == 0
            ? $"{total} {actionLabel}"
            : $"{total} {actionLabel} ({string.Join(", ", kindSummary)})";
    }

    private static string FormatCount(int count, string singular, string plural)
    {
        var noun = count == 1 ? singular : plural;
        return $"{count} {noun}";
    }

    private static bool TryParseTableScope(
        string value,
        out string schemaName,
        out string tableName)
    {
        schemaName = string.Empty;
        tableName = string.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var parts = value.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return false;
        }

        schemaName = parts[0];
        tableName = parts[1];
        return true;
    }

    private static bool TryParseColumnScope(
        string value,
        out string schemaName,
        out string tableName,
        out string columnName)
    {
        schemaName = string.Empty;
        tableName = string.Empty;
        columnName = string.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var parts = value.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 3)
        {
            return false;
        }

        schemaName = parts[0];
        tableName = parts[1];
        columnName = parts[2];
        return true;
    }

    private static List<MetaSqlDestructiveApproval> LoadApprovalsFromFile(string approvalFilePath)
    {
        var absolutePath = Path.GetFullPath(approvalFilePath);
        if (!File.Exists(absolutePath))
        {
            throw new InvalidOperationException($"approval file was not found at '{absolutePath}'.");
        }

        var json = File.ReadAllText(absolutePath);
        var payload = JsonSerializer.Deserialize<DestructiveApprovalsFile>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        if (payload is null)
        {
            throw new InvalidOperationException($"approval file '{absolutePath}' is empty or invalid.");
        }

        var result = new List<MetaSqlDestructiveApproval>();
        AddTableApprovals(result, payload.DataDropTable, MetaSqlDestructiveApprovalKind.DataDropTable);
        AddColumnApprovals(result, payload.DataDropColumn, MetaSqlDestructiveApprovalKind.DataDropColumn);
        AddColumnApprovals(result, payload.DataTruncationColumn, MetaSqlDestructiveApprovalKind.DataTruncationColumn);
        return result;
    }

    private static void AddTableApprovals(
        List<MetaSqlDestructiveApproval> approvals,
        IReadOnlyList<string>? values,
        MetaSqlDestructiveApprovalKind kind)
    {
        if (values is null)
        {
            return;
        }

        foreach (var value in values)
        {
            if (!TryParseTableScope(value, out var schemaName, out var tableName))
            {
                throw new InvalidOperationException(
                    $"invalid table scope '{value}' in approval file. Expected <schema>.<table>.");
            }

            approvals.Add(new MetaSqlDestructiveApproval
            {
                Kind = kind,
                SchemaName = schemaName,
                TableName = tableName,
            });
        }
    }

    private static void AddColumnApprovals(
        List<MetaSqlDestructiveApproval> approvals,
        IReadOnlyList<string>? values,
        MetaSqlDestructiveApprovalKind kind)
    {
        if (values is null)
        {
            return;
        }

        foreach (var value in values)
        {
            if (!TryParseColumnScope(value, out var schemaName, out var tableName, out var columnName))
            {
                throw new InvalidOperationException(
                    $"invalid column scope '{value}' in approval file. Expected <schema>.<table>.<column>.");
            }

            approvals.Add(new MetaSqlDestructiveApproval
            {
                Kind = kind,
                SchemaName = schemaName,
                TableName = tableName,
                ColumnName = columnName,
            });
        }
    }

    private sealed class DestructiveApprovalsFile
    {
        public List<string>? DataDropTable { get; init; }
        public List<string>? DataDropColumn { get; init; }
        public List<string>? DataTruncationColumn { get; init; }
    }

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
