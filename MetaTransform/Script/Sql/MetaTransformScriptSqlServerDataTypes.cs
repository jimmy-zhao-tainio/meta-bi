namespace MetaTransformScript.Sql;

internal static class MetaTransformScriptSqlServerDataTypes
{
    private static readonly IReadOnlyDictionary<string, string> SqlNameToOption =
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

    private static readonly IReadOnlyDictionary<string, string> OptionToSqlName =
        SqlNameToOption.ToDictionary(static pair => pair.Value, static pair => pair.Key, StringComparer.OrdinalIgnoreCase);

    public static bool TryMapSqlName(string sqlName, out string option) =>
        SqlNameToOption.TryGetValue(sqlName, out option!);

    public static string RenderSqlName(string option) =>
        OptionToSqlName.TryGetValue(option, out var sqlName)
            ? sqlName
            : throw new InvalidOperationException($"Unsupported SqlDataTypeOption '{option}'.");
}
