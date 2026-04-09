using MetaDataType.Instance;

namespace MetaSql;

internal static class SqlServerRenderingSupport
{
    private const string SqlServerDataTypeSystemId = "SqlServer";
    private static readonly IReadOnlyDictionary<string, (string DataTypeSystemId, string Name)> DataTypesById =
        MetaDataTypeInstance.Default.DataTypeList.ToDictionary(
            row => row.Id,
            row => (row.DataTypeSystemId, row.Name),
            StringComparer.Ordinal);

    internal static readonly HashSet<string> LengthBasedSqlServerTypeNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "varchar",
        "char",
        "nvarchar",
        "nchar",
        "varbinary",
        "binary",
    };

    public static string FormatTableName(Table table)
    {
        if (table.Schema is null)
        {
            throw new InvalidOperationException($"Table '{table.Id}' is missing Schema relationship.");
        }

        return $"{EscapeSqlIdentifier(table.Schema.Name)}.{EscapeSqlIdentifier(table.Name)}";
    }

    public static string EscapeSqlIdentifier(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return "[" + name.Replace("]", "]]", StringComparison.Ordinal) + "]";
    }

    public static bool IsTrue(string? value)
    {
        return string.Equals(value?.Trim(), "true", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsFalse(string? value)
    {
        return string.Equals(value?.Trim(), "false", StringComparison.OrdinalIgnoreCase);
    }

    public static string BuildSqlServerTypeSql(string metaDataTypeId, IReadOnlyDictionary<string, string> detailValues)
    {
        var typeName = GetRequiredSqlServerTypeName(metaDataTypeId);

        return typeName.ToLowerInvariant() switch
        {
            "char" or "varchar" or "nchar" or "nvarchar" or "binary" or "varbinary" =>
                $"{typeName}({NormalizeLength(RequireDetail(detailValues, "Length", metaDataTypeId))})",
            "decimal" or "numeric" =>
                BuildNumericTypeSql(typeName, detailValues, metaDataTypeId),
            "time" or "datetime2" or "datetimeoffset" =>
                detailValues.TryGetValue("Precision", out var precision) && !string.IsNullOrWhiteSpace(precision)
                    ? $"{typeName}({precision})"
                    : typeName,
            _ => typeName
        };
    }

    public static string RequireDetail(
        IReadOnlyDictionary<string, string> detailValues,
        string detailName,
        string metaDataTypeId)
    {
        if (!detailValues.TryGetValue(detailName, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"MetaDataTypeId '{metaDataTypeId}' requires detail '{detailName}'.");
        }

        return value;
    }

    public static bool IsSqlServerTypeId(string metaDataTypeId)
    {
        if (string.IsNullOrWhiteSpace(metaDataTypeId) ||
            !DataTypesById.TryGetValue(metaDataTypeId, out var dataType))
        {
            return false;
        }

        return string.Equals(dataType.DataTypeSystemId, SqlServerDataTypeSystemId, StringComparison.Ordinal);
    }

    public static string GetSqlServerTypeName(string metaDataTypeId)
    {
        if (string.IsNullOrWhiteSpace(metaDataTypeId) ||
            !DataTypesById.TryGetValue(metaDataTypeId, out var dataType) ||
            !string.Equals(dataType.DataTypeSystemId, SqlServerDataTypeSystemId, StringComparison.Ordinal))
        {
            return string.Empty;
        }

        return dataType.Name;
    }

    private static string GetRequiredSqlServerTypeName(string metaDataTypeId)
    {
        if (string.IsNullOrWhiteSpace(metaDataTypeId) ||
            !DataTypesById.TryGetValue(metaDataTypeId, out var dataType) ||
            !string.Equals(dataType.DataTypeSystemId, SqlServerDataTypeSystemId, StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(dataType.Name))
        {
            throw new InvalidOperationException(
                $"Unsupported MetaDataTypeId '{metaDataTypeId}'. Deploy only supports MetaDataType values under DataTypeSystem 'SqlServer'.");
        }

        return dataType.Name;
    }

    private static string BuildNumericTypeSql(
        string typeName,
        IReadOnlyDictionary<string, string> detailValues,
        string metaDataTypeId)
    {
        var precision = RequireDetail(detailValues, "Precision", metaDataTypeId);
        if (!detailValues.TryGetValue("Scale", out var scale) || string.IsNullOrWhiteSpace(scale))
        {
            return $"{typeName}({precision})";
        }

        return $"{typeName}({precision},{scale})";
    }

    private static string NormalizeLength(string value)
    {
        return string.Equals(value, "-1", StringComparison.Ordinal) ? "max" : value;
    }
}
