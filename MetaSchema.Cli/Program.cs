using System.Linq;
using Meta.Core.Presentation;
using Meta.Core.Services;
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

        var workspacePath = Path.GetFullPath(parseResult.Request.NewWorkspacePath);
        if (string.IsNullOrWhiteSpace(parseResult.Request.ConnectionString))
        {
            return Fail("missing required option --connection <connectionString>.", "meta-schema extract sqlserver --help");
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

        if (Directory.Exists(workspacePath) && Directory.EnumerateFileSystemEntries(workspacePath).Any())
        {
            return Fail($"target directory '{workspacePath}' must be empty.", "choose a new folder or empty the target directory and retry.", 4);
        }

        Directory.CreateDirectory(workspacePath);

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

        Presenter.WriteOk(
            "metaschema workspace created",
            ("Path", workspacePath),
            ("Model", workspace.Model.Name),
            ("Systems", workspace.Instance.GetOrCreateEntityRecords("System").Count.ToString()),
            ("Schemas", workspace.Instance.GetOrCreateEntityRecords("Schema").Count.ToString()),
            ("Tables", workspace.Instance.GetOrCreateEntityRecords("Table").Count.ToString()),
            ("Fields", workspace.Instance.GetOrCreateEntityRecords("Field").Count.ToString()),
            ("TableRelationships", workspace.Instance.GetOrCreateEntityRecords("TableRelationship").Count.ToString()),
            ("TableRelationshipFields", workspace.Instance.GetOrCreateEntityRecords("TableRelationshipField").Count.ToString()),
            ("DataTypeIds", workspace.Instance.GetOrCreateEntityRecords("Field")
                .Select(record => record.Values.TryGetValue("DataTypeId", out var typeId) ? typeId : string.Empty)
                .Where(typeId => !string.IsNullOrWhiteSpace(typeId))
                .Distinct(StringComparer.Ordinal)
                .Count().ToString()));
        return 0;
    }

    private static (bool Ok, SqlServerExtractRequest Request, string ErrorMessage) ParseSqlServerExtractOptions(
        string[] args,
        int startIndex)
    {
        var request = new SqlServerExtractRequest();
        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, request, "missing value for --new-workspace.");
                }

                request.NewWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--connection", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, request, "missing value for --connection.");
                }

                request.ConnectionString = args[++i];
                continue;
            }

            if (string.Equals(arg, "--schema", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, request, "missing value for --schema.");
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
                    return (false, request, "missing value for --system.");
                }

                request.SystemName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--table", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    return (false, request, "missing value for --table.");
                }

                request.TableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--all-tables", StringComparison.OrdinalIgnoreCase))
            {
                request.AllTables = true;
                continue;
            }

            return (false, request, $"unknown option '{arg}'.");
        }

        return (true, request, string.Empty);
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
        Presenter.WriteUsage("meta-schema extract sqlserver --new-workspace <path> --connection <connectionString> --system <name> (--schema <name> | --all-schemas) (--table <name> | --all-tables)");
        Presenter.WriteInfo("Notes:");
        Presenter.WriteInfo("  Creates a new workspace with the MetaSchema model and validates it.");
        Presenter.WriteInfo("  Scope is controlled by schema/table filters or all-schemas/all-tables discovery switches.");
        Presenter.WriteInfo("  TableRelationship rows are emitted only for enforced and trusted SQL Server foreign keys.");
        Presenter.WriteInfo("  Field rows carry a scalar DataTypeId such as sqlserver:type:nvarchar.");
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

