namespace MetaSql;

public enum MetaSqlDestructiveApprovalKind
{
    DataDropTable = 0,
    DataDropColumn,
    DataTruncationColumn,
}

public sealed record MetaSqlDestructiveApproval
{
    public required MetaSqlDestructiveApprovalKind Kind { get; init; }
    public required string SchemaName { get; init; }
    public required string TableName { get; init; }
    public string? ColumnName { get; init; }
}

public sealed class MetaSqlDestructiveApprovalSet
{
    private readonly HashSet<string> dataDropTableKeys;
    private readonly HashSet<string> dataDropColumnKeys;
    private readonly HashSet<string> dataTruncationColumnKeys;

    private MetaSqlDestructiveApprovalSet(
        HashSet<string> dataDropTableKeys,
        HashSet<string> dataDropColumnKeys,
        HashSet<string> dataTruncationColumnKeys)
    {
        this.dataDropTableKeys = dataDropTableKeys;
        this.dataDropColumnKeys = dataDropColumnKeys;
        this.dataTruncationColumnKeys = dataTruncationColumnKeys;
    }

    public static MetaSqlDestructiveApprovalSet Empty { get; } = new(
        new HashSet<string>(StringComparer.OrdinalIgnoreCase),
        new HashSet<string>(StringComparer.OrdinalIgnoreCase),
        new HashSet<string>(StringComparer.OrdinalIgnoreCase));

    public static MetaSqlDestructiveApprovalSet From(
        IReadOnlyList<MetaSqlDestructiveApproval>? approvals)
    {
        if (approvals is null || approvals.Count == 0)
        {
            return Empty;
        }

        var tableKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var dropColumnKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var truncateColumnKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var approval in approvals)
        {
            ArgumentNullException.ThrowIfNull(approval);

            var schemaName = NormalizeRequiredName(approval.SchemaName, nameof(approval.SchemaName));
            var tableName = NormalizeRequiredName(approval.TableName, nameof(approval.TableName));
            var columnName = approval.ColumnName?.Trim();

            switch (approval.Kind)
            {
                case MetaSqlDestructiveApprovalKind.DataDropTable:
                    tableKeys.Add(BuildTableKey(schemaName, tableName));
                    break;
                case MetaSqlDestructiveApprovalKind.DataDropColumn:
                    dropColumnKeys.Add(BuildColumnKey(schemaName, tableName, NormalizeRequiredName(columnName, nameof(approval.ColumnName))));
                    break;
                case MetaSqlDestructiveApprovalKind.DataTruncationColumn:
                    truncateColumnKeys.Add(BuildColumnKey(schemaName, tableName, NormalizeRequiredName(columnName, nameof(approval.ColumnName))));
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported destructive approval kind '{approval.Kind}'.");
            }
        }

        return new MetaSqlDestructiveApprovalSet(tableKeys, dropColumnKeys, truncateColumnKeys);
    }

    public bool HasDataDropTable(string schemaName, string tableName)
    {
        return dataDropTableKeys.Contains(BuildTableKey(schemaName, tableName));
    }

    public bool HasDataDropColumn(string schemaName, string tableName, string columnName)
    {
        return dataDropColumnKeys.Contains(BuildColumnKey(schemaName, tableName, columnName));
    }

    public bool HasDataTruncationColumn(string schemaName, string tableName, string columnName)
    {
        return dataTruncationColumnKeys.Contains(BuildColumnKey(schemaName, tableName, columnName));
    }

    public static string BuildTableKey(string schemaName, string tableName)
    {
        return NormalizeRequiredName(schemaName, nameof(schemaName)) + "." +
               NormalizeRequiredName(tableName, nameof(tableName));
    }

    public static string BuildColumnKey(string schemaName, string tableName, string columnName)
    {
        return BuildTableKey(schemaName, tableName) + "." +
               NormalizeRequiredName(columnName, nameof(columnName));
    }

    private static string NormalizeRequiredName(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing required destructive approval field '{fieldName}'.");
        }

        return value.Trim();
    }
}
