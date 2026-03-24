namespace MetaSql;

/// <summary>
/// Renders SQL Server truncation statements used before approved narrowing alters.
/// </summary>
internal sealed class SqlServerTruncateColumnDataSqlRenderer
{
    public string BuildTruncateColumnDataSql(
        TableColumn sourceColumn,
        TableColumn liveColumn,
        IReadOnlyDictionary<string, List<TableColumnDataTypeDetail>> sourceDetailsByColumnId)
    {
        if (sourceColumn.TableId != liveColumn.TableId)
        {
            throw new InvalidOperationException(
                $"Cannot truncate column data because source table '{sourceColumn.TableId}' and live table '{liveColumn.TableId}' differ.");
        }

        if (sourceColumn.Name != liveColumn.Name)
        {
            throw new InvalidOperationException(
                $"Cannot truncate column data because source column name '{sourceColumn.Name}' and live column name '{liveColumn.Name}' differ.");
        }

        if (!SqlServerRenderingSupport.IsSqlServerTypeId(sourceColumn.MetaDataTypeId) ||
            !SqlServerRenderingSupport.IsSqlServerTypeId(liveColumn.MetaDataTypeId))
        {
            throw new InvalidOperationException(
                $"Cannot truncate column '{sourceColumn.Id}' because only sqlserver:type:* MetaDataTypeId values are supported.");
        }

        var sourceTypeName = SqlServerRenderingSupport.GetSqlServerTypeName(sourceColumn.MetaDataTypeId);
        var liveTypeName = SqlServerRenderingSupport.GetSqlServerTypeName(liveColumn.MetaDataTypeId);
        if (!string.Equals(sourceTypeName, liveTypeName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Cannot truncate column '{sourceColumn.Id}' because type-family transitions are blocked in this deploy slice ({liveTypeName} -> {sourceTypeName}).");
        }

        if (!SqlServerRenderingSupport.LengthBasedSqlServerTypeNames.Contains(sourceTypeName))
        {
            throw new InvalidOperationException(
                $"Cannot truncate column '{sourceColumn.Id}' because only length-based sqlserver types are supported.");
        }

        var detailValues = GetGroup(sourceDetailsByColumnId, sourceColumn.Id)
            .ToDictionary(row => row.Name, row => row.Value, StringComparer.OrdinalIgnoreCase);
        var targetLengthRaw = SqlServerRenderingSupport.RequireDetail(detailValues, "Length", sourceColumn.MetaDataTypeId);
        if (!int.TryParse(targetLengthRaw, out var targetLength) || targetLength <= 0)
        {
            throw new InvalidOperationException(
                $"Cannot truncate column '{sourceColumn.Id}' because source Length detail '{targetLengthRaw}' is invalid.");
        }

        var columnSql = SqlServerRenderingSupport.EscapeSqlIdentifier(liveColumn.Name);
        var expressionSql = sourceTypeName.ToLowerInvariant() switch
        {
            "varchar" or "char" or "nvarchar" or "nchar" => $"LEFT({columnSql}, {targetLength})",
            "varbinary" or "binary" => $"SUBSTRING({columnSql}, 1, {targetLength})",
            _ => throw new InvalidOperationException(
                $"Cannot truncate column '{sourceColumn.Id}' because sqlserver type '{sourceTypeName}' is unsupported.")
        };

        var maxBytes = string.Equals(sourceTypeName, "nvarchar", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(sourceTypeName, "nchar", StringComparison.OrdinalIgnoreCase)
            ? targetLength * 2
            : targetLength;

        return $"UPDATE {SqlServerRenderingSupport.FormatTableName(liveColumn.Table)} SET {columnSql} = {expressionSql} WHERE {columnSql} IS NOT NULL AND DATALENGTH({columnSql}) > {maxBytes};";
    }

    private static IReadOnlyList<T> GetGroup<T>(IReadOnlyDictionary<string, List<T>> groups, string key)
    {
        return groups.TryGetValue(key, out var bucket)
            ? bucket
            : Array.Empty<T>();
    }
}
