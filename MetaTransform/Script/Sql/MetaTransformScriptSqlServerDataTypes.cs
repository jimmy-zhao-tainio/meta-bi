using MetaDataType.Instance;

namespace MetaTransformScript.Sql;

internal static class MetaTransformScriptSqlServerDataTypes
{
    private const string SqlServerDataTypeSystemId = "SqlServer";

    private static readonly IReadOnlyDictionary<string, string> CanonicalSqlNameToOption = BuildCanonicalSqlNameToOption();

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
            ["sysname"] = "nvarchar"
        };

    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> SqlNameAliasDefaultParameters =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["sysname"] = ["128"]
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

    public static string RenderSqlName(string option) =>
        OptionToSqlName.TryGetValue(option, out var sqlName)
            ? sqlName
            : throw new InvalidOperationException($"Unsupported SqlDataTypeOption '{option}'.");

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

    private static IReadOnlyDictionary<string, string> BuildCanonicalSqlNameToOption()
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var sanctionedSqlServerTypeNames = MetaDataTypeInstance.Default.DataTypeList
            .Where(dataType =>
                string.Equals(dataType.DataTypeSystemId, SqlServerDataTypeSystemId, StringComparison.Ordinal) &&
                !string.IsNullOrWhiteSpace(dataType.Name))
            .Select(dataType => dataType.Name.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var sqlName in sanctionedSqlServerTypeNames)
        {
            if (!TryMapCanonicalSqlNameToOption(sqlName, out var option))
            {
                throw new InvalidOperationException(
                    $"SqlServer data type '{sqlName}' is sanctioned in MetaDataType but is not mapped to a MetaTransformScript SqlDataTypeOption.");
            }

            map[NormalizeSqlTypeName(sqlName)] = option;
        }

        return map;
    }

    private static IReadOnlyDictionary<string, string> BuildSqlNameToOption()
    {
        var map = new Dictionary<string, string>(CanonicalSqlNameToOption, StringComparer.OrdinalIgnoreCase);
        foreach (var alias in SqlNameAliasToCanonicalSqlName)
        {
            if (!CanonicalSqlNameToOption.TryGetValue(alias.Value, out var mappedOption))
            {
                throw new InvalidOperationException(
                    $"SqlServer type alias '{alias.Key}' points to canonical SQL type '{alias.Value}', but that canonical type is not sanctioned in MetaDataType.");
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

    private static bool TryMapCanonicalSqlNameToOption(string sqlName, out string option)
    {
        option = sqlName.Trim().ToLowerInvariant() switch
        {
            "bigint" => "BigInt",
            "binary" => "Binary",
            "bit" => "Bit",
            "char" => "Char",
            "date" => "Date",
            "datetime" => "DateTime",
            "datetime2" => "DateTime2",
            "datetimeoffset" => "DateTimeOffset",
            "decimal" => "Decimal",
            "float" => "Float",
            "geography" => "Geography",
            "geometry" => "Geometry",
            "hierarchyid" => "HierarchyId",
            "int" => "Int",
            "money" => "Money",
            "nchar" => "NChar",
            "numeric" => "Numeric",
            "nvarchar" => "NVarChar",
            "real" => "Real",
            "smallint" => "SmallInt",
            "smallmoney" => "SmallMoney",
            "sql_variant" => "SqlVariant",
            "time" => "Time",
            "tinyint" => "TinyInt",
            "uniqueidentifier" => "UniqueIdentifier",
            "varbinary" => "VarBinary",
            "varchar" => "VarChar",
            "xml" => "Xml",
            _ => string.Empty
        };

        return !string.IsNullOrWhiteSpace(option);
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
