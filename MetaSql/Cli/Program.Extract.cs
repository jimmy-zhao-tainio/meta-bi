using Meta.Core.Connections;
using Meta.Core.Domain;
using Meta.Core.Presentation.Cli;
using Meta.Core.Services;
using MetaSql.Extractors.SqlServer;

internal static partial class Program
{
    private static CliCommandDefinition CreateExtractSqlServerCommand() =>
        new(
            "extract sqlserver",
            "Extract SQL Server database objects into a MetaSql workspace.",
            new[] { "meta-sql extract sqlserver --new-workspace <path> --connection-env <name> [--schema <name>] [--table <name>] [--include-tables] [--include-views] [--include-functions] [--include-stored-procedures] [--allow-empty]" },
            new[]
            {
                new CliOptionDefinition("--new-workspace <path>", "Required. Directory where the MetaSql workspace will be created."),
                new CliOptionDefinition("--connection-env <name>", "Required. Environment variable containing the SQL Server connection string."),
                new CliOptionDefinition("--schema <name>", "Optional. Extract only one SQL Server schema."),
                new CliOptionDefinition("--table <name>", "Optional. Extract only one table. SQL module extraction is skipped when a table filter is used."),
                new CliOptionDefinition("--include-tables", "Extract tables, columns, keys, and indexes. If no include switch is provided, all object kinds are extracted."),
                new CliOptionDefinition("--include-views", "Extract view modules. If any include switch is provided, only selected object kinds are extracted."),
                new CliOptionDefinition("--include-functions", "Extract function modules. If any include switch is provided, only selected object kinds are extracted."),
                new CliOptionDefinition("--include-stored-procedures", "Extract stored procedure modules. If any include switch is provided, only selected object kinds are extracted."),
                new CliOptionDefinition("--allow-empty", "Create an empty database/schema workspace when no tables or modules match.")
            },
            new[]
            {
                "Extracts deployable MetaSql state from SQL Server: tables, columns, primary keys, foreign keys, indexes, views, functions, and stored procedures.",
                "FunctionKind is derived from SQL Server object type: ScalarFunction, InlineTableValuedFunction, or TableValuedFunction.",
                "This is deployment-state import. Syntax-modeled CREATE VIEW/FUNCTION import remains owned by MetaTransformScript."
            },
            ShowInCommandCatalog: false);

    private static Task<int> RunExtractAsync(string[] args)
    {
        if (args.Length == 1 || IsHelpToken(args[1]))
        {
            PrintCommandHelp("extract");
            return Task.FromResult(0);
        }

        if (!string.Equals(args[1], "sqlserver", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(Fail($"unknown extractor '{args[1]}'.", HelpCommand("extract")));
        }

        if (args.Length >= 3 && IsHelpToken(args[2]))
        {
            PrintCommandHelp("extract sqlserver");
            return Task.FromResult(0);
        }

        var parse = ParseSqlServerExtractOptions(args, startIndex: 2);
        if (!parse.Ok)
        {
            return Task.FromResult(Fail(parse.ErrorMessage, HelpCommand("extract sqlserver")));
        }

        if (string.IsNullOrWhiteSpace(parse.Request.NewWorkspacePath))
        {
            return Task.FromResult(Fail("missing required option --new-workspace <path>.", HelpCommand("extract sqlserver")));
        }

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parse.Request.NewWorkspacePath);
        if (!targetValidation.Ok)
        {
            return Task.FromResult(Fail(targetValidation.ErrorMessage, "choose a new folder or empty the target directory and retry.", 4, targetValidation.Details));
        }

        if (string.IsNullOrWhiteSpace(parse.ConnectionEnvironmentVariableName))
        {
            return Task.FromResult(Fail("missing required option --connection-env <name>.", HelpCommand("extract sqlserver")));
        }

        try
        {
            parse.Request.ConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
                parse.ConnectionEnvironmentVariableName);
        }
        catch (ConnectionEnvironmentVariableException exception)
        {
            return Task.FromResult(Fail(
                "Cannot extract SQL database.",
                "set the named connection environment variable and retry.",
                4,
                [$"  {exception.Message}"]));
        }

        parse.Request.NewWorkspacePath = targetValidation.FullPath;
        Workspace workspace;
        try
        {
            workspace = new SqlServerMetaSqlExtractor().ExtractMetaSqlWorkspace(parse.Request);
        }
        catch (InvalidOperationException exception)
        {
            return Task.FromResult(Fail(
                "Cannot extract SQL database.",
                HelpCommand("extract sqlserver"),
                4,
                [$"  {exception.Message}"]));
        }

        var validation = new ValidationService().Validate(workspace);
        if (validation.HasErrors)
        {
            return Task.FromResult(Fail(
                "extracted MetaSql workspace is invalid.",
                "fix extractor mapping and retry extract.",
                4,
                validation.Issues
                    .Where(item => item.Severity == IssueSeverity.Error)
                    .Select(item => $"  - {item.Code}: {item.Message}")));
        }

        Presenter.WriteInfo($"Extracted {Path.GetFileName(targetValidation.FullPath)}");
        Presenter.WriteKeyValueBlock("Summary", new[]
        {
            ("Workspace", targetValidation.FullPath),
            ("Schemas", CountRecords(workspace, "Schema").ToString()),
            ("Tables", CountRecords(workspace, "Table").ToString()),
            ("Views", CountRecords(workspace, "View").ToString()),
            ("Functions", CountRecords(workspace, "Function").ToString()),
            ("StoredProcedures", CountRecords(workspace, "StoredProcedure").ToString()),
        });
        return Task.FromResult(0);
    }

    private static (bool Ok, SqlServerExtractRequest Request, string ConnectionEnvironmentVariableName, string ErrorMessage) ParseSqlServerExtractOptions(
        string[] args,
        int startIndex)
    {
        var request = new SqlServerExtractRequest();
        var connectionEnvironmentVariableName = string.Empty;
        var includeSwitchProvided = false;
        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, request, connectionEnvironmentVariableName, "missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(request.NewWorkspacePath)) return (false, request, connectionEnvironmentVariableName, "--new-workspace can only be provided once.");
                request.NewWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, request, connectionEnvironmentVariableName, "missing value for --connection-env.");
                if (!string.IsNullOrWhiteSpace(connectionEnvironmentVariableName)) return (false, request, connectionEnvironmentVariableName, "--connection-env can only be provided once.");
                connectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--schema", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, request, connectionEnvironmentVariableName, "missing value for --schema.");
                if (!string.IsNullOrWhiteSpace(request.SchemaName)) return (false, request, connectionEnvironmentVariableName, "--schema can only be provided once.");
                request.SchemaName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--table", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, request, connectionEnvironmentVariableName, "missing value for --table.");
                if (!string.IsNullOrWhiteSpace(request.TableName)) return (false, request, connectionEnvironmentVariableName, "--table can only be provided once.");
                request.TableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--include-tables", StringComparison.OrdinalIgnoreCase))
            {
                AddObjectKindFilter(ref includeSwitchProvided, request, SqlServerExtractObjectKinds.Tables);
                continue;
            }

            if (string.Equals(arg, "--include-views", StringComparison.OrdinalIgnoreCase))
            {
                AddObjectKindFilter(ref includeSwitchProvided, request, SqlServerExtractObjectKinds.Views);
                continue;
            }

            if (string.Equals(arg, "--include-functions", StringComparison.OrdinalIgnoreCase))
            {
                AddObjectKindFilter(ref includeSwitchProvided, request, SqlServerExtractObjectKinds.Functions);
                continue;
            }

            if (string.Equals(arg, "--include-stored-procedures", StringComparison.OrdinalIgnoreCase))
            {
                AddObjectKindFilter(ref includeSwitchProvided, request, SqlServerExtractObjectKinds.StoredProcedures);
                continue;
            }

            if (string.Equals(arg, "--allow-empty", StringComparison.OrdinalIgnoreCase))
            {
                request.AllowEmpty = true;
                continue;
            }

            return (false, request, connectionEnvironmentVariableName, $"unknown option '{arg}'.");
        }

        if (!string.IsNullOrWhiteSpace(request.TableName))
        {
            if (!includeSwitchProvided)
            {
                request.ObjectKinds = SqlServerExtractObjectKinds.Tables;
            }
            else if (request.ObjectKinds != SqlServerExtractObjectKinds.Tables)
            {
                return (false, request, connectionEnvironmentVariableName, "--table can only be used with --include-tables.");
            }
        }

        return (true, request, connectionEnvironmentVariableName, string.Empty);
    }

    private static void AddObjectKindFilter(
        ref bool includeSwitchProvided,
        SqlServerExtractRequest request,
        SqlServerExtractObjectKinds objectKind)
    {
        if (!includeSwitchProvided)
        {
            request.ObjectKinds = SqlServerExtractObjectKinds.None;
            includeSwitchProvided = true;
        }

        request.ObjectKinds |= objectKind;
    }

    private static int CountRecords(Workspace workspace, string entityName)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName).Count;
    }
}
