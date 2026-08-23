using System.Text.Json;
using Meta.Core.Presentation;
using MetaCli.Core;
using MetaSql;
using MetaSqlDeployManifest;

internal sealed partial class MetaSqlCommandHandlers
{
    private readonly ConsolePresenter Presenter;
    private readonly string appName;

    public MetaSqlCommandHandlers(ConsolePresenter presenter, string appName)
    {
        Presenter = presenter;
        this.appName = appName;
    }

    private string HelpCommand(string commandName) => $"{appName} help {commandName}";

    private int Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
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

    private List<MetaSqlDestructiveApproval> BuildDestructiveApprovals(MetaCliInvocation invocation)
    {
        var approvals = new List<MetaSqlDestructiveApproval>();
        foreach (var value in invocation.Values("approve-drop-table"))
        {
            if (!TryParseTableScope(value, out var schemaName, out var tableName))
            {
                throw new MetaCliExitException(Fail(
                    $"invalid table scope '{value}' for --approve-drop-table. Expected <schema>.<table>.",
                    HelpCommand("deploy-plan")));
            }

            approvals.Add(new MetaSqlDestructiveApproval
            {
                Kind = MetaSqlDestructiveApprovalKind.DataDropTable,
                SchemaName = schemaName,
                TableName = tableName,
            });
        }

        foreach (var value in invocation.Values("approve-drop-column"))
        {
            if (!TryParseColumnScope(value, out var schemaName, out var tableName, out var columnName))
            {
                throw new MetaCliExitException(Fail(
                    $"invalid column scope '{value}' for --approve-drop-column. Expected <schema>.<table>.<column>.",
                    HelpCommand("deploy-plan")));
            }

            approvals.Add(new MetaSqlDestructiveApproval
            {
                Kind = MetaSqlDestructiveApprovalKind.DataDropColumn,
                SchemaName = schemaName,
                TableName = tableName,
                ColumnName = columnName,
            });
        }

        foreach (var value in invocation.Values("approve-truncate-column"))
        {
            if (!TryParseColumnScope(value, out var schemaName, out var tableName, out var columnName))
            {
                throw new MetaCliExitException(Fail(
                    $"invalid column scope '{value}' for --approve-truncate-column. Expected <schema>.<table>.<column>.",
                    HelpCommand("deploy-plan")));
            }

            approvals.Add(new MetaSqlDestructiveApproval
            {
                Kind = MetaSqlDestructiveApprovalKind.DataTruncationColumn,
                SchemaName = schemaName,
                TableName = tableName,
                ColumnName = columnName,
            });
        }

        var approvalFilePath = invocation.Optional("approval-file");
        if (!string.IsNullOrWhiteSpace(approvalFilePath))
        {
            try
            {
                approvals.AddRange(LoadApprovalsFromFile(approvalFilePath));
            }
            catch (Exception ex) when (ex is InvalidOperationException or JsonException)
            {
                throw new MetaCliExitException(Fail(ex.Message, HelpCommand("deploy-plan")));
            }
        }

        return approvals;
    }

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
}
