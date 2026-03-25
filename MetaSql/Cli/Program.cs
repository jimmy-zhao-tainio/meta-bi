using Meta.Core.Presentation;
using MetaSql;
using MetaSql.Extractors.SqlServer;
using System.Text.Json;

internal static partial class Program
{
    private static readonly ConsolePresenter Presenter = new();

    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return 0;
        }

        if (string.Equals(args[0], "deploy-plan", StringComparison.OrdinalIgnoreCase))
        {
            return await RunDeployPlanAsync(args).ConfigureAwait(false);
        }

        if (string.Equals(args[0], "deploy", StringComparison.OrdinalIgnoreCase))
        {
            return await RunDeployAsync(args).ConfigureAwait(false);
        }

        return Fail($"unknown command '{args[0]}'.", "meta-sql help");
    }

    private static (bool Ok, string SourceWorkspacePath, string OutputPath, string ConnectionString, IReadOnlyList<MetaSqlDestructiveApproval> DestructiveApprovals, string ErrorMessage) ParseDiffLikeArgs(string[] args, int startIndex)
    {
        var sourceWorkspacePath = string.Empty;
        var outputPath = string.Empty;
        var connectionString = string.Empty;
        string? approvalFilePath = null;
        var explicitApprovals = new List<MetaSqlDestructiveApproval>();

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--source-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, "missing value for --source-workspace.");
                if (!string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, "--source-workspace can only be provided once.");
                sourceWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--out", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, "missing value for --out.");
                if (!string.IsNullOrWhiteSpace(outputPath)) return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, "--out can only be provided once.");
                outputPath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--connection-string", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, "missing value for --connection-string.");
                if (!string.IsNullOrWhiteSpace(connectionString)) return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, "--connection-string can only be provided once.");
                connectionString = args[++i];
                continue;
            }

            if (string.Equals(arg, "--approve-drop-table", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, "missing value for --approve-drop-table.");
                var value = args[++i];
                if (!TryParseTableScope(value, out var approvedSchema, out var approvedTable))
                {
                    return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, $"invalid table scope '{value}' for --approve-drop-table. Expected <schema>.<table>.");
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
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, "missing value for --approve-drop-column.");
                var value = args[++i];
                if (!TryParseColumnScope(value, out var approvedSchema, out var approvedTable, out var approvedColumn))
                {
                    return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, $"invalid column scope '{value}' for --approve-drop-column. Expected <schema>.<table>.<column>.");
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
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, "missing value for --approve-truncate-column.");
                var value = args[++i];
                if (!TryParseColumnScope(value, out var approvedSchema, out var approvedTable, out var approvedColumn))
                {
                    return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, $"invalid column scope '{value}' for --approve-truncate-column. Expected <schema>.<table>.<column>.");
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
                if (i + 1 >= args.Length) return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, "missing value for --approval-file.");
                if (!string.IsNullOrWhiteSpace(approvalFilePath)) return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, "--approval-file can only be provided once.");
                approvalFilePath = args[++i];
                continue;
            }

            return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, "missing required option --source-workspace <path>.");
        if (string.IsNullOrWhiteSpace(connectionString)) return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, "missing required option --connection-string <value>.");

        if (!string.IsNullOrWhiteSpace(approvalFilePath))
        {
            try
            {
                var fileApprovals = LoadApprovalsFromFile(approvalFilePath);
                explicitApprovals.AddRange(fileApprovals);
            }
            catch (Exception ex)
            {
                return (false, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, ex.Message);
            }
        }

        return (true, sourceWorkspacePath, outputPath, connectionString, explicitApprovals, string.Empty);
    }

    private static (bool Ok, string ManifestWorkspacePath, string SourceWorkspacePath, string ConnectionString, string ErrorMessage) ParseDeployArgs(string[] args, int startIndex)
    {
        var manifestWorkspacePath = string.Empty;
        var sourceWorkspacePath = string.Empty;
        var connectionString = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--manifest-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionString, "missing value for --manifest-workspace.");
                if (!string.IsNullOrWhiteSpace(manifestWorkspacePath)) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionString, "--manifest-workspace can only be provided once.");
                manifestWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--source-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionString, "missing value for --source-workspace.");
                if (!string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionString, "--source-workspace can only be provided once.");
                sourceWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--connection-string", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionString, "missing value for --connection-string.");
                if (!string.IsNullOrWhiteSpace(connectionString)) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionString, "--connection-string can only be provided once.");
                connectionString = args[++i];
                continue;
            }

            return (false, manifestWorkspacePath, sourceWorkspacePath, connectionString, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(manifestWorkspacePath)) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionString, "missing required option --manifest-workspace <path>.");
        if (string.IsNullOrWhiteSpace(sourceWorkspacePath)) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionString, "missing required option --source-workspace <path>.");
        if (string.IsNullOrWhiteSpace(connectionString)) return (false, manifestWorkspacePath, sourceWorkspacePath, connectionString, "missing required option --connection-string <value>.");

        return (true, manifestWorkspacePath, sourceWorkspacePath, connectionString, string.Empty);
    }

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        Presenter.WriteUsage("meta-sql <command> [options]");
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteCommandCatalog(
            "Commands:",
            new[]
            {
                ("help", "Show this help."),
                ("deploy-plan", "Create a deploy manifest (add/alter/block/replace; destructive actions require exact object-scoped approvals)."),
                ("deploy", "Apply a deploy manifest after source/live fingerprint validation.")
            });
        Presenter.WriteInfo(string.Empty);
        Presenter.WriteNext("meta-sql deploy-plan --help");
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
