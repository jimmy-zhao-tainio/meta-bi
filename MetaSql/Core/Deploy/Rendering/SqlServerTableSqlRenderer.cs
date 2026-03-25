namespace MetaSql;

/// <summary>
/// Renders SQL Server table and column create/add/drop statements.
/// </summary>
internal sealed class SqlServerTableSqlRenderer
{
    public string BuildDropColumnSql(TableColumn column)
    {
        return $"ALTER TABLE {SqlServerRenderingSupport.FormatTableName(column.Table)} DROP COLUMN {SqlServerRenderingSupport.EscapeSqlIdentifier(column.Name)};";
    }

    public string BuildDropTableSql(Table table)
    {
        return $"DROP TABLE {SqlServerRenderingSupport.FormatTableName(table)};";
    }

    public string BuildCreateTableSql(
        Table table,
        IReadOnlyList<TableColumn> columns,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> detailsByColumnId)
    {
        var definitions = columns.Select(row => BuildColumnDefinition(row, detailsByColumnId));
        return $"CREATE TABLE {SqlServerRenderingSupport.FormatTableName(table)} ({string.Join(", ", definitions)});";
    }

    public string BuildAddColumnSql(
        TableColumn column,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> detailsByColumnId)
    {
        return $"ALTER TABLE {SqlServerRenderingSupport.FormatTableName(column.Table)} ADD {BuildColumnDefinition(column, detailsByColumnId)};";
    }

    private static string BuildColumnDefinition(
        TableColumn column,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> detailsByColumnId)
    {
        if (!string.IsNullOrWhiteSpace(column.ExpressionSql))
        {
            return $"{SqlServerRenderingSupport.EscapeSqlIdentifier(column.Name)} AS {column.ExpressionSql}";
        }

        var detailValues = GetGroup(detailsByColumnId, column.Id)
            .ToDictionary(row => row.Name, row => row.Value, StringComparer.OrdinalIgnoreCase);
        var typeSql = SqlServerRenderingSupport.BuildSqlServerTypeSql(column.MetaDataTypeId, detailValues);
        var identitySql = SqlServerRenderingSupport.IsTrue(column.IsIdentity)
            ? $" IDENTITY({NormalizeIdentityValue(column.IdentitySeed, "1")},{NormalizeIdentityValue(column.IdentityIncrement, "1")})"
            : string.Empty;
        var nullableSql = SqlServerRenderingSupport.IsTrue(column.IsNullable) ? "NULL" : "NOT NULL";
        return $"{SqlServerRenderingSupport.EscapeSqlIdentifier(column.Name)} {typeSql}{identitySql} {nullableSql}";
    }

    private static IReadOnlyList<T> GetGroup<T>(IReadOnlyDictionary<string, List<T>> groups, string key)
    {
        return groups.TryGetValue(key, out var bucket)
            ? bucket
            : Array.Empty<T>();
    }

    private static string NormalizeIdentityValue(string? value, string defaultValue)
    {
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
    }
}
