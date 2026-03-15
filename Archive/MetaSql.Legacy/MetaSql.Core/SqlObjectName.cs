namespace MetaSql.Core;

internal static class SqlObjectName
{
    public static string Format(string schemaName, string objectName)
    {
        return $"{schemaName}.{objectName}";
    }
}
