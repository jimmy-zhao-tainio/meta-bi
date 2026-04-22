using System.Linq;
using Meta.Core.Connections;
using Meta.Core.Presentation;
using Meta.Core.Services;
using MetaBi.Cli.Common;
using MetaSchema.Core;
using MetaSchema.Extractors.SqlServer;

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

        if (string.Equals(args[0], "extract", StringComparison.OrdinalIgnoreCase))
        {
            return await RunExtractAsync(args).ConfigureAwait(false);
        }

        return Fail($"unknown command '{args[0]}'.", "meta-schema help");
    }

    private static async Task<int> RunExtractAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintExtractHelp();
            return 0;
        }

        if (!string.Equals(args[1], "sqlserver", StringComparison.OrdinalIgnoreCase))
        {
            return Fail($"unknown extractor '{args[1]}'.", "meta-schema extract --help");
        }

        if (args.Length >= 3 && IsHelpToken(args[2]))
        {
            PrintExtractSqlServerHelp();
            return 0;
        }

        var parseResult = ParseSqlServerExtractOptions(args, startIndex: 2);
        if (!parseResult.Ok)
        {
            return Fail(parseResult.ErrorMessage, "meta-schema extract sqlserver --help");
        }

        if (string.IsNullOrWhiteSpace(parseResult.Request.NewWorkspacePath))
        {
            return Fail("missing required option --new-workspace <path>.", "meta-schema extract sqlserver --help");
        }

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parseResult.Request.NewWorkspacePath);
        if (!targetValidation.Ok)
        {
            return Fail(targetValidation.ErrorMessage, "choose a new folder or empty the target directory and retry.", 4, targetValidation.Details);
        }

        var workspacePath = targetValidation.FullPath;
        if (string.IsNullOrWhiteSpace(parseResult.ConnectionEnvironmentVariableName))
        {
            return Fail("missing required option --connection-env <name>.", "meta-schema extract sqlserver --help");
        }

        if (string.IsNullOrWhiteSpace(parseResult.Request.SystemName))
        {
            return Fail("missing required option --system <name>.", "meta-schema extract sqlserver --help");
        }

        if (!string.IsNullOrWhiteSpace(parseResult.Request.SchemaName) && parseResult.Request.AllSchemas)
        {
            return Fail("--schema and --all-schemas cannot be used together.", "meta-schema extract sqlserver --help");
        }

        if (string.IsNullOrWhiteSpace(parseResult.Request.SchemaName) && !parseResult.Request.AllSchemas)
        {
            return Fail("missing required scope option --schema <name> or --all-schemas.", "meta-schema extract sqlserver --help");
        }

        if (!string.IsNullOrWhiteSpace(parseResult.Request.TableName) && parseResult.Request.AllTables)
        {
            return Fail("--table and --all-tables cannot be used together.", "meta-schema extract sqlserver --help");
        }

        if (string.IsNullOrWhiteSpace(parseResult.Request.TableName) && !parseResult.Request.AllTables)
        {
            return Fail("missing required scope option --table <name> or --all-tables.", "meta-schema extract sqlserver --help");
        }

        try
        {
            parseResult.Request.ConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
                parseResult.ConnectionEnvironmentVariableName);
        }
        catch (ConnectionEnvironmentVariableException exception)
        {
            return Fail(exception.Message, "meta-schema extract sqlserver --help");
        }

        Meta.Core.Domain.Workspace workspace;
        try
        {
            workspace = new SqlServerSchemaExtractor().ExtractMetaSchemaWorkspace(parseResult.Request);
        }
        catch (InvalidOperationException exception)
        {
            return Fail(exception.Message, "meta-schema extract sqlserver --help", 4);
        }

        var validation = new ValidationService().Validate(workspace);
        if (validation.HasErrors)
        {
            return Fail(
                "extracted schema workspace is invalid.",
                "fix extractor mapping and retry extract.",
                4,
                validation.Issues
                    .Where(item => item.Severity == Meta.Core.Domain.IssueSeverity.Error)
                    .Select(item => $"  - {item.Code}: {item.Message}"));
        }

        await new WorkspaceService().SaveAsync(workspace).ConfigureAwait(false);

        Presenter.WriteOk($"Created {Path.GetFileName(workspacePath)}");
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
        Presenter.WriteUsage("meta-schema <command> [options]");
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog(
            "Commands:",
            new[]
            {
                ("help", "Show this help."),
                ("extract", "Materialize sanctioned MetaSchema workspaces from external sources.")
            });
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteNext("meta-schema extract --help");
    }

    private static void PrintExtractHelp()
    {
        Presenter.WriteInfo("Command: extract");
        Presenter.WriteUsage("meta-schema extract <extractor> [options]");
        Presenter.WriteCommandCatalog(
            "Extractors",
            new[]
            {
                ("sqlserver", "Extract SQL Server schema into MetaSchema workspace.")
            });
        Presenter.WriteNext("meta-schema extract sqlserver --help");
    }

    private static void PrintExtractSqlServerHelp()
    {
        Presenter.WriteInfo("Command: extract sqlserver");
        Presenter.WriteUsage("meta-schema extract sqlserver --new-workspace <path> --connection-env <name> --system <name> (--schema <name> | --all-schemas) (--table <name> | --all-tables)");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Creates a new workspace with the MetaSchema model and validates it.");
        Presenter.WriteInfo("  --connection-env names the environment variable that contains the SQL Server connection string.");
        Presenter.WriteInfo("  Scope is controlled by schema/table filters or all-schemas/all-tables discovery switches.");
        Presenter.WriteInfo("  TableRelationship rows are emitted only for enforced and trusted SQL Server foreign keys whose source and target tables are both in scope.");
        Presenter.WriteInfo("  Field rows carry a scalar DataTypeId plus local FieldDataTypeDetail rows such as Length, Precision, or Scale.");
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

