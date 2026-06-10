using System.Linq;
using Meta.Core.Connections;
using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using Meta.Core.Services;
using MetaSchema.Core;
using MetaSchema.Extractors.SqlServer;

internal static class Program
{
    private const string AppName = "meta-schema";

    private static readonly ConsolePresenter Presenter = new();
    private static readonly IReadOnlyList<CliCommandRoute> CommandRoutes = BuildCommandRoutes();
    private static readonly IReadOnlyDictionary<string, CliCommandRoute> CommandRoutesByName = CommandRoutes
        .ToDictionary(route => route.Definition.Name, StringComparer.OrdinalIgnoreCase);
    private static readonly CliAppDefinition Cli = new(
        AppName,
        new[]
        {
            "meta-schema <command> [options]"
        },
        CommandRoutes.Select(route => route.Definition).Concat(new[] { CreateExtractSqlServerCommand() }).ToArray(),
        Next: "meta-schema extract --help");

    internal static CliAppDefinition CreateAppDefinition() => Cli;

    private static IReadOnlyList<CliCommandRoute> BuildCommandRoutes() =>
        new[]
        {
            new CliCommandRoute(
                new CliCommandDefinition(
                    "help",
                    "Show this help.",
                    new[] { "meta-schema help" }),
                _ =>
                {
                    PrintHelp();
                    return Task.FromResult(0);
                }),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "extract",
                    "Materialize sanctioned MetaSchema workspaces from external sources.",
                    new[] { "meta-schema extract <extractor> [options]" },
                    Notes: new[] { "Available extractor: sqlserver." },
                    Next: "meta-schema extract sqlserver --help"),
                RunExtractAsync)
        };

    private static CliCommandDefinition CreateExtractSqlServerCommand() =>
        new(
            "extract sqlserver",
            "Extract SQL Server schema into MetaSchema workspace.",
            new[] { "meta-schema extract sqlserver --new-workspace <path> --connection-env <name> --system <name> (--schema <name> | --all-schemas) (--table <name> | --all-tables)" },
            new[]
            {
                new CliOptionDefinition("--new-workspace <path>", "Required. Directory where the MetaSchema workspace will be created."),
                new CliOptionDefinition("--connection-env <name>", "Required. Environment variable containing the SQL Server connection string."),
                new CliOptionDefinition("--system <name>", "Required. Source system name recorded in the workspace."),
                new CliOptionDefinition("--schema <name>", "Extract one SQL Server schema. Mutually exclusive with --all-schemas."),
                new CliOptionDefinition("--all-schemas", "Extract all SQL Server schemas in scope."),
                new CliOptionDefinition("--table <name>", "Extract one SQL Server table or view. Mutually exclusive with --all-tables."),
                new CliOptionDefinition("--all-tables", "Extract all SQL Server tables and views in scope.")
            },
            new[]
            {
                "Creates a new workspace with the MetaSchema model and validates it.",
                "Scope is controlled by schema/table filters or all-schemas/all-tables discovery switches.",
                "TableRelationship rows are emitted only for enforced and trusted SQL Server foreign keys whose source and target tables are both in scope.",
                "Field rows carry a scalar DataTypeId plus local FieldDataTypeDetail rows such as Length, Precision, or Scale."
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

    private static async Task<int> RunExtractAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintCommandHelp("extract");
            return 0;
        }

        if (!string.Equals(args[1], "sqlserver", StringComparison.OrdinalIgnoreCase))
        {
            return Fail($"unknown extractor '{args[1]}'.", HelpCommand("extract"));
        }

        if (args.Length >= 3 && IsHelpToken(args[2]))
        {
            PrintCommandHelp("extract sqlserver");
            return 0;
        }

        var parseResult = ParseSqlServerExtractOptions(args, startIndex: 2);
        if (!parseResult.Ok)
        {
            return Fail(parseResult.ErrorMessage, HelpCommand("extract sqlserver"));
        }

        if (string.IsNullOrWhiteSpace(parseResult.Request.NewWorkspacePath))
        {
            return Fail("missing required option --new-workspace <path>.", HelpCommand("extract sqlserver"));
        }

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parseResult.Request.NewWorkspacePath);
        if (!targetValidation.Ok)
        {
            return Fail(targetValidation.ErrorMessage, "choose a new folder or empty the target directory and retry.", 4, targetValidation.Details);
        }

        var workspacePath = targetValidation.FullPath;
        if (string.IsNullOrWhiteSpace(parseResult.ConnectionEnvironmentVariableName))
        {
            return Fail("missing required option --connection-env <name>.", HelpCommand("extract sqlserver"));
        }

        if (string.IsNullOrWhiteSpace(parseResult.Request.SystemName))
        {
            return Fail("missing required option --system <name>.", HelpCommand("extract sqlserver"));
        }

        if (!string.IsNullOrWhiteSpace(parseResult.Request.SchemaName) && parseResult.Request.AllSchemas)
        {
            return Fail("--schema and --all-schemas cannot be used together.", HelpCommand("extract sqlserver"));
        }

        if (string.IsNullOrWhiteSpace(parseResult.Request.SchemaName) && !parseResult.Request.AllSchemas)
        {
            return Fail("missing required scope option --schema <name> or --all-schemas.", HelpCommand("extract sqlserver"));
        }

        if (!string.IsNullOrWhiteSpace(parseResult.Request.TableName) && parseResult.Request.AllTables)
        {
            return Fail("--table and --all-tables cannot be used together.", HelpCommand("extract sqlserver"));
        }

        if (string.IsNullOrWhiteSpace(parseResult.Request.TableName) && !parseResult.Request.AllTables)
        {
            return Fail("missing required scope option --table <name> or --all-tables.", HelpCommand("extract sqlserver"));
        }

        try
        {
            parseResult.Request.ConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
                parseResult.ConnectionEnvironmentVariableName);
        }
        catch (ConnectionEnvironmentVariableException exception)
        {
            return Fail(
                "Cannot extract schema.",
                "set the named connection environment variable and retry.",
                4,
                [$"  {exception.Message}"]);
        }

        using var activity = CliActivityLine.Start("Extracting");
        Meta.Core.Domain.Workspace workspace;
        try
        {
            workspace = new SqlServerSchemaExtractor().ExtractMetaSchemaWorkspace(parseResult.Request);
        }
        catch (InvalidOperationException exception)
        {
            activity.Dispose();
            return Fail(
                "Cannot extract schema.",
                HelpCommand("extract sqlserver"),
                4,
                [$"  {exception.Message}"]);
        }

        var validation = new ValidationService().Validate(workspace);
        if (validation.HasErrors)
        {
            activity.Dispose();
            return Fail(
                "extracted schema workspace is invalid.",
                "fix extractor mapping and retry extract.",
                4,
                validation.Issues
                    .Where(item => item.Severity == Meta.Core.Domain.IssueSeverity.Error)
                    .Select(item => $"  - {item.Code}: {item.Message}"));
        }

        await new WorkspaceService().SaveAsync(workspace).ConfigureAwait(false);

        activity.Succeed();
        return 0;
    }

    private static (bool Ok, SqlServerExtractRequest Request, string ConnectionEnvironmentVariableName, string ErrorMessage) ParseSqlServerExtractOptions(
        string[] args,
        int startIndex)
    {
        var request = new SqlServerExtractRequest();
        var connectionEnvironmentVariableName = string.Empty;
        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, request, connectionEnvironmentVariableName, "missing value for --new-workspace.");
                }

                if (!string.IsNullOrWhiteSpace(request.NewWorkspacePath))
                {
                    return (false, request, connectionEnvironmentVariableName, "--new-workspace can only be provided once.");
                }

                request.NewWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, request, connectionEnvironmentVariableName, "missing value for --connection-env.");
                }

                if (!string.IsNullOrWhiteSpace(connectionEnvironmentVariableName))
                {
                    return (false, request, connectionEnvironmentVariableName, "--connection-env can only be provided once.");
                }

                connectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--schema", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, request, connectionEnvironmentVariableName, "missing value for --schema.");
                }

                if (!string.IsNullOrWhiteSpace(request.SchemaName))
                {
                    return (false, request, connectionEnvironmentVariableName, "--schema can only be provided once.");
                }

                request.SchemaName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--all-schemas", StringComparison.OrdinalIgnoreCase))
            {
                request.AllSchemas = true;
                continue;
            }

            if (string.Equals(arg, "--system", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, request, connectionEnvironmentVariableName, "missing value for --system.");
                }

                if (!string.IsNullOrWhiteSpace(request.SystemName))
                {
                    return (false, request, connectionEnvironmentVariableName, "--system can only be provided once.");
                }

                request.SystemName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--table", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, request, connectionEnvironmentVariableName, "missing value for --table.");
                }

                if (!string.IsNullOrWhiteSpace(request.TableName))
                {
                    return (false, request, connectionEnvironmentVariableName, "--table can only be provided once.");
                }

                request.TableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--all-tables", StringComparison.OrdinalIgnoreCase))
            {
                request.AllTables = true;
                continue;
            }

            return (false, request, connectionEnvironmentVariableName, $"unknown option '{arg}'.");
        }

        return (true, request, connectionEnvironmentVariableName, string.Empty);
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

    private static void PrintExtractHelp()
    {
        PrintCommandHelp("extract");
    }

    private static void PrintExtractSqlServerHelp()
    {
        PrintCommandHelp("extract sqlserver");
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
