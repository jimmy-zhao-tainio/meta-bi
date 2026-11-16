namespace MetaTransformScript.Sql;

public static class MetaTransformScriptSqlServerDataTypes
{
    // Native SQL syntax maps to the bounded options understood by the parser and emitter.
    // Workspace catalogs resolve semantic type identities during binding, not syntax parsing.
    private static readonly IReadOnlyDictionary<string, string> CanonicalSqlNameToOption =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["bigint"] = "BigInt",
            ["binary"] = "Binary",
            ["bit"] = "Bit",
            ["char"] = "Char",
            ["date"] = "Date",
            ["datetime"] = "DateTime",
            ["datetime2"] = "DateTime2",
            ["datetimeoffset"] = "DateTimeOffset",
            ["decimal"] = "Decimal",
            ["float"] = "Float",
            ["geography"] = "Geography",
            ["geometry"] = "Geometry",
            ["hierarchyid"] = "HierarchyId",
            ["int"] = "Int",
            ["money"] = "Money",
            ["nchar"] = "NChar",
            ["numeric"] = "Numeric",
            ["nvarchar"] = "NVarChar",
            ["real"] = "Real",
            ["smallint"] = "SmallInt",
            ["smallmoney"] = "SmallMoney",
            ["sql_variant"] = "SqlVariant",
            ["time"] = "Time",
            ["tinyint"] = "TinyInt",
            ["uniqueidentifier"] = "UniqueIdentifier",
            ["varbinary"] = "VarBinary",
            ["varchar"] = "VarChar",
            ["xml"] = "Xml"
        };

    // SQL Server's native spelling aliases and built-in alias defaults.
    private static readonly IReadOnlyDictionary<string, string> SqlNameAliasToCanonicalSqlName =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["char varying"] = "varchar",
            ["character varying"] = "varchar",
            ["dec"] = "decimal",
            ["double precision"] = "float",
            ["integer"] = "int",
            ["national char"] = "nchar",
            ["national character"] = "nchar",
            ["national char varying"] = "nvarchar",
            ["national character varying"] = "nvarchar",
            ["rowversion"] = "binary",
            ["timestamp"] = "binary",
            ["sysname"] = "nvarchar"
        };

    // Preserve the parser's existing application-alias behavior. These are not native SQL types
    // or catalog discovery; supporting arbitrary application aliases is a separate syntax contract.
    private static readonly IReadOnlyDictionary<string, string> LegacyApplicationAliases =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["AccountNumber"] = "nvarchar",
            ["Flag"] = "bit",
            ["Name"] = "nvarchar",
            ["NameStyle"] = "bit",
            ["OrderNumber"] = "nvarchar",
            ["Phone"] = "nvarchar"
        };

    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> SqlNameAliasDefaultParameters =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["rowversion"] = ["8"],
            ["sysname"] = ["128"],
            ["timestamp"] = ["8"]
        };

    private static readonly IReadOnlyDictionary<string, string> SqlNameToOption = BuildSqlNameToOption();
    private static readonly IReadOnlyDictionary<string, string> OptionToSqlName = BuildOptionToSqlName();

    public static bool TryMapSqlName(string sqlName, out string option)
    {
        if (string.IsNullOrWhiteSpace(sqlName))
        {
            option = string.Empty;
            return false;
        }

        return SqlNameToOption.TryGetValue(NormalizeSqlTypeName(sqlName), out option!);
    }

    public static string RenderSqlName(string? option)
    {
        if (string.IsNullOrWhiteSpace(option))
        {
            throw new InvalidOperationException("SqlDataTypeOption is required.");
        }

        return OptionToSqlName.TryGetValue(option, out var sqlName)
            ? sqlName
            : throw new InvalidOperationException($"Unsupported SqlDataTypeOption '{option}'.");
    }

    public static IReadOnlyList<string> GetDefaultParametersForSqlName(string sqlName)
    {
        if (string.IsNullOrWhiteSpace(sqlName))
        {
            return [];
        }

        var normalizedSqlName = NormalizeSqlTypeName(sqlName);
        foreach (var pair in SqlNameAliasDefaultParameters)
        {
            if (string.Equals(NormalizeSqlTypeName(pair.Key), normalizedSqlName, StringComparison.Ordinal))
            {
                return pair.Value;
            }
        }

        return [];
    }

    private static IReadOnlyDictionary<string, string> BuildSqlNameToOption()
    {
        var map = new Dictionary<string, string>(CanonicalSqlNameToOption, StringComparer.OrdinalIgnoreCase);
        foreach (var alias in SqlNameAliasToCanonicalSqlName.Concat(LegacyApplicationAliases))
        {
            if (!CanonicalSqlNameToOption.TryGetValue(alias.Value, out var mappedOption))
            {
                throw new InvalidOperationException(
                    $"SQL type alias '{alias.Key}' points to unsupported native SQL type '{alias.Value}'.");
            }

            map[NormalizeSqlTypeName(alias.Key)] = mappedOption;
        }

        return map;
    }

    private static IReadOnlyDictionary<string, string> BuildOptionToSqlName()
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in CanonicalSqlNameToOption)
        {
            map[pair.Value] = pair.Key;
        }

        return map;
    }

    private static string NormalizeSqlTypeName(string sqlName)
    {
        var trimmed = sqlName.Trim();
        if (trimmed.Length == 0)
        {
            return string.Empty;
        }

        var builder = new System.Text.StringBuilder(trimmed.Length);
        var previousWasWhitespace = false;
        foreach (var ch in trimmed)
        {
            if (char.IsWhiteSpace(ch))
            {
                if (!previousWasWhitespace)
                {
                    builder.Append(' ');
                    previousWasWhitespace = true;
                }

                continue;
            }

            builder.Append(char.ToLowerInvariant(ch));
            previousWasWhitespace = false;
        }

        return builder.ToString();
    }
}
