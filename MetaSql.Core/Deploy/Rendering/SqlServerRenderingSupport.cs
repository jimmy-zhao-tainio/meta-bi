namespace MetaSql;

internal static class SqlServerRenderingSupport
{
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
        const string prefix = "sqlserver:type:";
        if (string.IsNullOrWhiteSpace(metaDataTypeId) ||
            !metaDataTypeId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Unsupported MetaDataTypeId '{metaDataTypeId}'. Deploy only supports sqlserver:type:*.");
        }

        var typeName = metaDataTypeId[prefix.Length..];
        if (string.IsNullOrWhiteSpace(typeName))
        {
            throw new InvalidOperationException(
                $"Unsupported MetaDataTypeId '{metaDataTypeId}'. Deploy only supports sqlserver:type:*.");
        }

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
        return !string.IsNullOrWhiteSpace(metaDataTypeId) &&
               metaDataTypeId.StartsWith("sqlserver:type:", StringComparison.OrdinalIgnoreCase);
    }

    public static string GetSqlServerTypeName(string metaDataTypeId)
    {
        const string prefix = "sqlserver:type:";
        if (string.IsNullOrWhiteSpace(metaDataTypeId) ||
            !metaDataTypeId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        return metaDataTypeId[prefix.Length..];
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
